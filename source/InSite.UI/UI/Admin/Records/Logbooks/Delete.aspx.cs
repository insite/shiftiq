using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!DeleteJournalsCheckBox.Checked && DeleteJournalsCheckBox.Visible)
                DeleteButton.Attributes["disabled"] = "disabled";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(
                    JournalSetupIdentifier,
                    x => x.Event,
                    x => x.Achievement,
                    x => x.Framework,
                    x => x.Fields,
                    x => x.CompetencyRequirements
                );

                PageHelper.AutoBindHeader(this, null, journalSetup.JournalSetupName);

                if (journalSetup == null || journalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }

                BindImpact(journalSetup);
                LogbookDetail.BindLogbook(journalSetup.JournalSetupIdentifier, journalSetup, User.TimeZone, false);

                CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}";
            }
        }

        private void BindImpact(QJournalSetup journalSetup)
        {
            var journalCount = ServiceLocator.JournalSearch.CountJournals(new QJournalFilter { JournalSetupIdentifier = JournalSetupIdentifier });
            var userCount = ServiceLocator.JournalSearch.CountJournalSetupUsers(new VJournalSetupUserFilter
            {
                JournalSetupIdentifier = JournalSetupIdentifier,
                Role = JournalSetupUserRole.Learner
            });

            FieldCount.Text = $"{journalSetup.Fields.Count:n0}";
            CompetencyCount.Text = $"{journalSetup.CompetencyRequirements.Count:n0}";
            JournalCount.Text = $"{journalCount:n0}";
            UserCount.Text = $"{userCount:n0}";

            NoVoid.Visible = journalCount > 0;

            DeleteJournalsCheckBox.Visible = journalCount > 0;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!DeleteJournalsCheckBox.Checked && DeleteJournalsCheckBox.Visible)
                return;

            var commands = new List<Command>();

            var journals = ServiceLocator.JournalSearch.GetJournals(new QJournalFilter { JournalSetupIdentifier = JournalSetupIdentifier });
            foreach (var journal in journals)
                commands.Add(new DeleteJournal(journal.JournalIdentifier));

            commands.Add(new DeleteJournalSetup(JournalSetupIdentifier));

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}"
                : null;
        }

        /*
        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }
        */
    }
}