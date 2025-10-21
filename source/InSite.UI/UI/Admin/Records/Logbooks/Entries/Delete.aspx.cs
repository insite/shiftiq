using System;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Journals.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Records.Logbooks.Entries
{
    public partial class Delete : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;

        private Guid JournalSetupIdentifier
        {
            get => (Guid)ViewState[nameof(JournalSetupIdentifier)];
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        private Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

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
                LoadData();

                CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
            }
        }

        private void LoadData()
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier,
                x => x.Journal.JournalSetup,
                x => x.Journal.JournalSetup.Event,
                x => x.Journal.JournalSetup.Achievement,
                x => x.Journal.JournalSetup.Framework
                );

            if (experience == null || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                return;
            }

            var journalSetup = experience.Journal.JournalSetup;

            JournalSetupIdentifier = journalSetup.JournalSetupIdentifier;
            UserIdentifier = experience.Journal.UserIdentifier;

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);

            PageHelper.AutoBindHeader(this);

            EntryNumber.Text = experience.Sequence.ToString();
            EntryCreated.Text = TimeZones.Format(experience.ExperienceCreated, User.TimeZone, true);

            // LogbookDetail.BindLogbook(journalSetup.JournalSetupIdentifier, journalSetup, User.TimeZone, false);

            var person = PersonSearch.Select(Organization.Identifier, experience.Journal.UserIdentifier, x => x.User);
            LearnerName.Text = person.User.FullName;
            LearnerEmail.Text = person.User.Email;

            LogbookName.Text = journalSetup.JournalSetupName;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier);

            ServiceLocator.SendCommand(new DeleteExperience(experience.JournalIdentifier, experience.ExperienceIdentifier));

            HttpResponseHelper.Redirect(GetParentUrl(DefaultParameters));
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline-journal")
                ? DefaultParameters
                : GetParentLinkParameters(parent, null);
        }

        string DefaultParameters => $"journalsetup={JournalSetupIdentifier}&user={UserIdentifier}";
    }
}