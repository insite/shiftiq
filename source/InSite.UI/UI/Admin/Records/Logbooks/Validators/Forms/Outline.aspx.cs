using System;

using InSite.Admin.Records.Logbooks;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class Outline : AdminBasePage
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalsetup"], out var journalSetupIdentifier) ? journalSetupIdentifier : Guid.Empty;

        private int BulkAddedEntries => int.TryParse(Request.QueryString["bulk-added-entries"], out var value) ? value : 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);
            if (journalSetup == null
                || journalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier
                || !ServiceLocator.JournalSearch.IsLogbookValidator(JournalSetupIdentifier, User.UserIdentifier)
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");
                return;
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);

            PageHelper.AutoBindHeader(this, null, header);

            Users.LoadData(JournalSetupIdentifier, journalSetup.AchievementIdentifier.HasValue, null);

            if (Organization.Toolkits.Logbooks?.LogbookBulkEntry == true && BulkAddedEntries > 0)
            {
                var text = BulkAddedEntries == 1
                    ? "One entry was successfully added to the logbook"
                    : $"{BulkAddedEntries} entries were successfully added to the logbook";

                StatusAlert.AddMessage(AlertType.Success, text);
            }

            AddUsers.NavigateUrl = $"/ui/admin/records/logbooks/validators/add-users?journalsetup={JournalSetupIdentifier}";

            BulkAddEntriesButton.Visible = Organization.Toolkits.Logbooks?.LogbookBulkEntry == true;
            BulkAddEntriesButton.NavigateUrl = $"/ui/admin/records/logbooks/validators/bulk-entry?journalsetup={JournalSetupIdentifier}";
        }
    }
}
