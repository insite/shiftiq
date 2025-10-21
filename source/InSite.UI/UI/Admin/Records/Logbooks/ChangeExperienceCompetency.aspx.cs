using System;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

using ChangeExperienceCompetencyCommand = InSite.Application.Journals.Write.ChangeExperienceCompetency;

namespace InSite.Admin.Records.Logbooks
{
    public partial class ChangeExperienceCompetency : AdminBasePage, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;
        private Guid CompetencyIdentifier => Guid.TryParse(Request["competency"], out var value) ? value : Guid.Empty;

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

        private decimal? MaxHours
        {
            get => (decimal?)ViewState[nameof(MaxHours)];
            set => ViewState[nameof(MaxHours)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HoursValidator.ServerValidate += HoursValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var experience = ServiceLocator.JournalSearch.GetExperience(
                ExperienceIdentifier,
                x => x.Journal.JournalSetup.Event
            );

            if (experience == null || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
                RedirectToSearch();

            var experienceCompetency = ServiceLocator.JournalSearch
                .GetExperienceCompetency(ExperienceIdentifier, CompetencyIdentifier, x => x.Competency);

            if (experienceCompetency == null)
                RedirectToSearch();

            JournalSetupIdentifier = experience.Journal.JournalSetupIdentifier;
            UserIdentifier = experience.Journal.UserIdentifier;
            MaxHours = experience.ExperienceHours;

            var header = LogbookHeaderHelper.GetLogbookHeader(experience.Journal.JournalSetup, User.TimeZone);
            PageHelper.AutoBindHeader(this, null, header);

            var competency = experienceCompetency.Competency;
            var classification = "Competency";

            CompetencyTitle.Text = CompetencyHelper.GetStandardName(
                competency.CompetencyIdentifier,
                competency.CompetencyAsset,
                competency.CompetencyLabel,
                competency.CompetencyCode,
                classification
            );

            HoursValue.ValueAsDecimal = experienceCompetency.CompetencyHours;

            CancelButton.NavigateUrl = GetUrlToValidate();
        }

        private void HoursValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = MaxHours == null
                || decimal.TryParse(args.Value, out var hours) && hours <= MaxHours;

            if (args.IsValid)
                return;

            var template = Translate("Number of hours in competency should be less or equal to {0:n2}");

            HoursValidator.ErrorMessage = string.Format(template, MaxHours);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier);

            var command = new ChangeExperienceCompetencyCommand(experience.JournalIdentifier, ExperienceIdentifier, CompetencyIdentifier, HoursValue.ValueAsDecimal);

            ServiceLocator.SendCommand(command);

            RedirectToValidate();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search", true);

        private void RedirectToValidate() => HttpResponseHelper.Redirect(GetUrlToValidate(), true);

        private string GetUrlToValidate() => $"/ui/admin/records/logbooks/validate-experience?experience={ExperienceIdentifier}&panel=competencies";

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/validate-experience"))
                return $"experience={ExperienceIdentifier}&panel=competencies";

            if (parent.Name.EndsWith("/outline-journal"))
                return $"journalsetup={JournalSetupIdentifier}&user={UserIdentifier}";

            return null;
        }
    }
}
