using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Entries
{
    public partial class ViewExperience : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;

        private Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && !HttpRequestHelper.IsAjaxRequest)
            {
                if (Request["panel"] == "competencies")
                    CompetenciesPanel.IsSelected = true;

                LoadData();
            }
        }

        private void LoadData()
        {
            var returnUrl = new ReturnUrl();

            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier,
                x => x.Journal.JournalSetup,
                x => x.Journal.JournalSetup.Event,
                x => x.Journal.JournalSetup.Achievement,
                x => x.Journal.JournalSetup.Framework,
                x => x.Validator);
            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/entries/search");
                return;
            }

            if (experience.ExperienceValidated.HasValue)
            {
                ValidateButton.Icon = "fas fa-graduation-cap";
                ValidateButton.Text = "Validated";
                ValidateButton.Enabled = false;
            }

            ValidateButton.NavigateUrl = returnUrl
                .GetRedirectUrl($"/ui/admin/records/logbooks/validate-experience?experience={experience.ExperienceIdentifier}");

            var journalSetup = experience.Journal.JournalSetup;
            var content = ServiceLocator.ContentSearch.GetBlock(journalSetup.JournalSetupIdentifier, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default ?? journalSetup.JournalSetupName;

            JournalIdentifier = experience.JournalIdentifier;

            PageHelper.AutoBindHeader(this, null, title);

            var person = ServiceLocator.PersonSearch.GetPerson(experience.Journal.UserIdentifier, Organization.Identifier, x => x.User);
            PersonDetail.BindPerson(person, User.TimeZone);
            LogbookDetail.BindLogbook(journalSetup.JournalSetupIdentifier, journalSetup, User.TimeZone, false);

            Detail.LoadData(journalSetup.JournalSetupIdentifier, experience);

            LearnerLogbookPanel.Icon = experience.ExperienceValidated.HasValue
                ? "graduation-cap"
                : "book-open";

            var validator = experience.Validator != null ? experience.Validator.UserFullName : UserNames.Someone;

            ValidationStatus.Text = experience.ExperienceValidated.HasValue
                ? $"Validated by {validator}"
                : "Not Validated";

            if (experience.SkillRating.HasValue)
                SkillRating.Text = experience.SkillRating.ToString();
            else
                SkillRatingDiv.Visible = false;

            LoadCompetencies();
            LoadComments();
        }

        private bool LoadCompetencies()
        {
            var hasCompetencies = Competencies.LoadData(ExperienceIdentifier, false);
            CompetenciesPanel.Visible = hasCompetencies;
            return hasCompetencies;
        }

        private void LoadComments()
        {
            var journal = ServiceLocator.JournalSearch.GetJournal(JournalIdentifier, x => x.Experiences);
            CommentsPanel.Visible = Comments.LoadData(journal, ExperienceIdentifier);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);
    }
}
