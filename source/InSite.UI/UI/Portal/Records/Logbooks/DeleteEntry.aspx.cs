using System;

using InSite.Application.Journals.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class DeleteEntry : PortalBasePage, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var id) ? id : Guid.Empty;

        private Guid JournalSetupIdentifier
        {
            get => (Guid)ViewState[nameof(JournalSetupIdentifier)];
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        private Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

        private Guid? MediaEvidenceFileIdentifier
        {
            get => (Guid?)ViewState[nameof(MediaEvidenceFileIdentifier)];
            set => ViewState[nameof(MediaEvidenceFileIdentifier)] = value;
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
                LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (MediaEvidenceFileIdentifier.HasValue)
                ServiceLocator.StorageService.Delete(MediaEvidenceFileIdentifier.Value);

            ServiceLocator.SendCommand(new DeleteExperience(JournalIdentifier, ExperienceIdentifier));

            var url = $"/ui/portal/records/logbooks/outline?journalsetup={JournalSetupIdentifier}";
            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier, x => x.Journal.JournalSetup);

            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || experience.Journal.UserIdentifier != User.UserIdentifier
                || experience.ExperienceValidated.HasValue
                )
            {
                HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search");
                return;
            }

            var journalSetup = experience.Journal.JournalSetup;
            var content = ServiceLocator.ContentSearch.GetBlock(journalSetup.JournalSetupIdentifier);

            JournalSetupIdentifier = journalSetup.JournalSetupIdentifier;
            JournalIdentifier = experience.JournalIdentifier;
            MediaEvidenceFileIdentifier = experience.MediaEvidenceFileIdentifier;

            var competencyCount = ServiceLocator.JournalSearch.CountExperienceCompetencies(new QExperienceCompetencyFilter
            {
                ExperienceIdentifier = ExperienceIdentifier
            });
            CompetencyCount.Text = $"{competencyCount:n0}";

            Detail.LoadData(JournalSetupIdentifier, experience);

            CancelButton.NavigateUrl = $"/ui/portal/records/logbooks/outline?journalsetup={JournalSetupIdentifier}";

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text.Get(Identity.Language) ?? journalSetup.JournalSetupName);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/outline-entry"))
                return $"experience={ExperienceIdentifier}";

            if (parent.Name.EndsWith("/outline"))
                return $"journalsetup={JournalSetupIdentifier}";

            return null;
        }
    }
}