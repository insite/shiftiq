using System;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Credentials.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Achievements
{
    public partial class GrantAchievements : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/records/logbooks/search";

        protected Guid JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalsetup"], out var journalSetupIdentifier) ? journalSetupIdentifier : Guid.Empty;

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
                LoadData();

                CancelButton.NavigateUrl = GetBackUrl("users");
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Users.SelectedUsers.Count == 0)
            {
                ScreenAlert.AddMessage(AlertType.Error, "There are no selected learners");
                return;
            }

            Grant();

            HttpResponseHelper.Redirect(GetBackUrl("achievements"));
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null
                || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || journalSetup.AchievementIdentifier == null
                )
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var hasUsers = Users.LoadData(JournalSetupIdentifier, journalSetup.AchievementIdentifier.Value) > 0;

            if (!hasUsers)
            {
                MainPanel.Visible = false;
                SaveButton.Visible = false;

                ScreenAlert.AddMessage(AlertType.Warning, "There are no learners to grant achievements");
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);

            PageHelper.AutoBindHeader(Page, null, header);
        }

        private void Grant()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            
            if (journalSetup.AchievementIdentifier == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var achievement = journalSetup.AchievementIdentifier.Value;

            foreach (var user in Users.SelectedUsers)
            {
                if (ServiceLocator.AchievementSearch.GetCredential(user, achievement) != null)
                    continue;

                var person = PersonSearch.Select(Organization.Identifier, user, x => x.EmployerGroup);
                if (person == null)
                    continue;

                var command = new CreateAndGrantCredential(
                    Guid.NewGuid(),
                    Organization.Identifier,
                    achievement,
                    user,
                    DateTimeOffset.UtcNow,
                    $"Granted in the scope of the logbook {journalSetup.JournalSetupName}",
                    null,
                    person.EmployerGroupIdentifier,
                    person.EmployerGroup?.GroupStatus
                );

                ServiceLocator.SendCommand(command);
            }
        }

        private string GetBackUrl(string panel)
            => $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel={panel}";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=users"
                : null;
        }
    }
}