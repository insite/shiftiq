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
        private Guid JournalSetupID => Guid.TryParse(Request["journalsetup"], out var result) ? result : Guid.Empty;
        private Guid ValidatorID => Guid.TryParse(Request["user"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupID}&panel=setup";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupID,
                x => x.Event,
                x => x.Achievement,
                x => x.Framework,
                x => x.Fields,
                x => x.CompetencyRequirements);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search");
                return;
            }

            PageHelper.AutoBindHeader(this, null, LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone));

            var instructor = ServiceLocator.PersonSearch.GetPerson(ValidatorID, Organization.Key, x => x.User);

            PersonDetail.BindPerson(instructor, User.TimeZone);

            LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupID}\">{journalSetup.JournalSetupName}</a>";
            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupID, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default;
            LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteJournalSetupUser(JournalSetupID, ValidatorID, JournalSetupUserRole.Validator));

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupID}&panel=setup");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupID}&panel=setup"
                : null;
        }
    }
}