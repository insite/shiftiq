using System;

using InSite.Admin.Records.Logbooks;
using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Validators
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupId => Guid.TryParse(Request["journalsetup"], out var result) ? result : Guid.Empty;
        private Guid UserId => Guid.TryParse(Request["user"], out var result) ? result : Guid.Empty;
        private Guid GroupId => Guid.TryParse(Request["group"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupId}&panel=setup";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupId,
                x => x.Event,
                x => x.Achievement,
                x => x.Framework,
                x => x.Fields,
                x => x.CompetencyRequirements
            );
            if (journalSetup == null || journalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search");
                return;
            }

            PageHelper.AutoBindHeader(this, null, LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone));

            if (UserId != Guid.Empty)
            {
                var instructor = ServiceLocator.PersonSearch.GetPerson(UserId, Organization.Key, x => x.User);
                PersonDetail.Visible = true;
                PersonDetail.BindPerson(instructor, User.TimeZone);
            }
            else
            {
                var group = ServiceLocator.GroupSearch.GetGroup(GroupId);
                GroupDetail.Visible = true;
                GroupDetail.BindGroup(group);
            }

            LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupId}\">{journalSetup.JournalSetupName}</a>";
            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupId, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default;
            LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (UserId != Guid.Empty)
                ServiceLocator.SendCommand(new DeleteJournalSetupUser(JournalSetupId, UserId, JournalSetupUserRole.Validator));
            else
                ServiceLocator.SendCommand(new RemoveJournalSetupGroup(JournalSetupId, GroupId, JournalSetupUserRole.Validator));

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupId}&panel=setup");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupId}&panel=setup"
                : null;
        }
    }
}
