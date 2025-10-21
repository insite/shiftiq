using System;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks
{
    public partial class DeleteUser : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;
        private Guid UserIdentifier => Guid.TryParse(Request["user"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var user = ServiceLocator.JournalSearch.GetJournalSetupUser(JournalSetupIdentifier, UserIdentifier, JournalSetupUserRole.Learner,
                    x => x.JournalSetup,
                    x => x.JournalSetup.Event,
                    x => x.JournalSetup.Achievement,
                    x => x.JournalSetup.Framework);

                if (user == null || user.JournalSetup.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }

                LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}\">{user.JournalSetup.JournalSetupName}</a>";
                var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier, MultilingualString.DefaultLanguage);
                var title = content?.Title?.Text.Default;
                LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";

                PageHelper.AutoBindHeader(this, null, user.JournalSetup.JournalSetupName);

                LoadData();

                CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
            }
        }

        private void LoadData()
        {
            var entryCount = ServiceLocator.JournalSearch.CountExperiences(new QExperienceFilter { JournalSetupIdentifier = JournalSetupIdentifier, UserIdentifier = UserIdentifier });

            EntryCount.Text = $"{entryCount:0}";

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, Organization.Identifier, x => x.User);

            PersonDetail.BindPerson(person, User.TimeZone);

            if (entryCount > 0)
                DeleteConfirmationCheckbox.Text = "Delete Learner and all Learner's Log Entries";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteJournalSetupUser(JournalSetupIdentifier, UserIdentifier, JournalSetupUserRole.Learner));

            HttpResponseHelper.Redirect(GetParentUrl(DefaultParameters));
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? DefaultParameters
                : GetParentLinkParameters(parent, null);
        }

        string DefaultParameters => $"journalsetup={JournalSetupIdentifier}&panel=users";
    }
}