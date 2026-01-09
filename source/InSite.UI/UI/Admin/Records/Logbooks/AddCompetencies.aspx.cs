using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks
{
    public partial class AddCompetencies : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        protected Guid AreaIdentifier { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event, x => x.Framework);

                if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }

                if (journalSetup.Framework == null)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=competencies");
                    return;
                }

                var frameworkName = $"{journalSetup.Framework.FrameworkTitle} Asset #{journalSetup.Framework.FrameworkAsset}";
                FrameworkTitle.Text = $"<a href=\"/ui/admin/standards/edit?id={journalSetup.Framework.FrameworkIdentifier}\">{frameworkName} </a>";

                var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);

                PageHelper.AutoBindHeader(this, null, header);

                if (!LoadCompetencies(journalSetup.FrameworkStandardIdentifier.Value))
                {
                    NoCompetenciesAlert.AddMessage(AlertType.Warning, "All competencies have been added to logbook");

                    MainPanel.Visible = false;
                    SaveButton.Visible = false;
                }

                CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=competencies";
            }
        }

        private bool LoadCompetencies(Guid frameworkIdentifier)
        {
            var areas = CompetencyHelper.GetAreas(JournalSetupIdentifier, frameworkIdentifier, false);

            if (areas == null)
                return false;

            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas;
            AreaRepeater.DataBind();

            return true;
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (CompetencyHelper.AreaItem)e.Item.DataItem;

            AreaIdentifier = area.Identifier;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.DataBind();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var competencies = GetSelectedCompetencies();
            if (competencies.Count == 0)
            {
                EditorStatus.AddMessage(AlertType.Error, "There are no selected competencies");
                return;
            }

            var commands = new List<Command>();
            foreach (var competency in competencies)
                commands.Add(new AddCompetencyRequirement(JournalSetupIdentifier, competency, null, null, null, false));

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=competencies");
        }

        private List<Guid> GetSelectedCompetencies()
        {
            var list = new List<Guid>();

            foreach (RepeaterItem areaItem in AreaRepeater.Items)
            {
                var competencyRepeater = (Repeater)areaItem.FindControl("CompetencyRepeater");

                foreach (RepeaterItem competencyItem in competencyRepeater.Items)
                {
                    var selectedCheckBox = (ICheckBoxControl)competencyItem.FindControl("Selected");
                    if (!selectedCheckBox.Checked)
                        continue;

                    var identifierLiteral = (ITextControl)competencyItem.FindControl("Identifier");
                    var identifier = Guid.Parse(identifierLiteral.Text);

                    list.Add(identifier);
                }
            }

            return list;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=competencies"
                : null;
        }
    }
}
