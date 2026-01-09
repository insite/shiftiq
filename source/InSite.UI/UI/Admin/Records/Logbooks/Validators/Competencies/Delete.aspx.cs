using System;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Journals.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Competencies
{
    public partial class Delete : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;
        private Guid CompetencyIdentifier => Guid.TryParse(Request["competency"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var experienceCompetency = ServiceLocator.JournalSearch.GetExperienceCompetency(ExperienceIdentifier, CompetencyIdentifier, x => x.Experience);

            ServiceLocator.SendCommand(new DeleteExperienceCompetency(experienceCompetency.Experience.JournalIdentifier, ExperienceIdentifier, CompetencyIdentifier));

            HttpResponseHelper.Redirect(GetParentUrl(null));
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

            var journalSetup = experienceCompetency.Experience.Journal.JournalSetup;

            if (!ServiceLocator.JournalSearch.ExistsJournalSetupUser(journalSetup.JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator))
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/competencies/search");
                return;
            }

            PageHelper.AutoBindHeader(this, null, LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone));

            var learner = PersonSearch.Select(Organization.Key, experienceCompetency.Experience.Journal.UserIdentifier, x => x.User);

            Competency.LoadData(experienceCompetency);

            LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={journalSetup.JournalSetupIdentifier}\">{journalSetup.JournalSetupName}</a>";
            var content = ServiceLocator.ContentSearch.GetBlock(journalSetup.JournalSetupIdentifier, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default;
            LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";

            UserName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={learner.User.UserIdentifier}\">{learner.User.FullName}</a>";
            UserEmail.Text = $"<a href='mailto:{learner.User.Email}'>{learner.User.Email}</a>";
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);
    }
}