using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Competencies
{
    public partial class ViewExperience : AdminBasePage, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;
        private Guid CompetencyIdentifier => Guid.TryParse(Request["competency"], out var value) ? value : Guid.Empty;

        private Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && !HttpRequestHelper.IsAjaxRequest)
                LoadData();
        }

        private void LoadData()
        {
            var experienceCompetency = ServiceLocator.JournalSearch.GetExperienceCompetency(ExperienceIdentifier, CompetencyIdentifier,
                x => x.Experience.Journal.JournalSetup,
                x => x.Experience.Journal.JournalSetup.Event,
                x => x.Experience.Journal.JournalSetup.Achievement,
                x => x.Experience.Journal.JournalSetup.Framework,
                x => x.Competency);

            if (experienceCompetency == null
                || experienceCompetency.Experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/competencies/search");
                return;
            }

            if (!ServiceLocator.JournalSearch.ExistsJournalSetupUser(experienceCompetency.Experience.Journal.JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator))
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/competencies/search");
                return;
            }

            var experience = experienceCompetency.Experience;
            var journalSetup = experience.Journal.JournalSetup;
            var content = ServiceLocator.ContentSearch.GetBlock(journalSetup.JournalSetupIdentifier, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default ?? journalSetup.JournalSetupName;

            JournalIdentifier = experience.JournalIdentifier;

            PageHelper.AutoBindHeader(this, null, title);

            var person = ServiceLocator.PersonSearch.GetPerson(experience.Journal.UserIdentifier, Organization.Identifier, x => x.User);
            PersonDetail.BindPerson(person, User.TimeZone);
            LogbookDetail.BindLogbook(journalSetup.JournalSetupIdentifier, journalSetup, User.TimeZone, false);

            Detail.LoadData(journalSetup.JournalSetupIdentifier, experience);

            var validator = experience.Validator != null ? experience.Validator.UserFullName : UserNames.Someone;

            ValidationStatus.Text = experience.ExperienceValidated.HasValue
                ? $"Validated by {validator}"
                : "Not Validated";

            if (experience.SkillRating.HasValue)
                SkillRating.Text = experience.SkillRating.ToString();
            else
                SkillRatingDiv.Visible = false;

            Competency.LoadData(experienceCompetency);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline-journal")
                ? $"journal={JournalIdentifier}"
                : null;
        }
    }
}
