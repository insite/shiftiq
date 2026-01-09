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
        protected Guid JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalsetup"], out var journalSetupIdentifier) ? journalSetupIdentifier : Guid.Empty;

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
                || ServiceLocator.JournalSearch.GetJournalSetupUser(JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator) == null
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");
                return;
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);

            PageHelper.AutoBindHeader(this, null, header);

            Users.LoadData(JournalSetupIdentifier, journalSetup.AchievementIdentifier.HasValue, null);

            AddUsers.NavigateUrl = $"/ui/admin/records/logbooks/validators/add-users?journalsetup={JournalSetupIdentifier}";
        }
    }
}
