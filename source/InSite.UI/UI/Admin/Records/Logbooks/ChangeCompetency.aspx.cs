using System;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks
{
    public partial class ChangeCompetency : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;
        private Guid CompetencyIdentifier => Guid.TryParse(Request["competency"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var requirement = ServiceLocator.JournalSearch.GetCompetencyRequirement(JournalSetupIdentifier, CompetencyIdentifier, x => x.JournalSetup, x => x.JournalSetup.Event);
            if (requirement == null || requirement.JournalSetup.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                return;
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(requirement.JournalSetup, User.TimeZone);
            PageHelper.AutoBindHeader(this, null, header);

            LoadData(requirement);

            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={requirement.JournalSetupIdentifier}&panel=competencies";
        }

        private void LoadData(QCompetencyRequirement requirement)
        {
            var standard = StandardSearch.Select(requirement.CompetencyStandardIdentifier);

            CompetencyName.Text = $"<a href=\"/ui/admin/standards/edit?id={requirement.CompetencyStandardIdentifier}\">{CompetencyHelper.GetStandardName(standard)}</a>";
            CompetencyHours.ValueAsDecimal = requirement.CompetencyHours;
            JournalItems.ValueAsInt = requirement.JournalItems;
            SkillRating.ValueAsInt = requirement.SkillRating;
            IncludeHoursToArea.Checked = requirement.IncludeHoursToArea;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var hours = CompetencyHours.ValueAsDecimal;
            var journalItems = JournalItems.ValueAsInt;
            var skillRating = SkillRating.ValueAsInt;
            var includeHoursToArea = IncludeHoursToArea.Checked;

            ServiceLocator.SendCommand(new ChangeCompetencyRequirement(JournalSetupIdentifier, CompetencyIdentifier, hours, journalItems, skillRating, includeHoursToArea));

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=competencies");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=competencies"
                : null;
        }
    }
}
