using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

using ValidateExperienceCommand = InSite.Application.Journals.Write.ValidateExperience;

namespace InSite.Admin.Records.Logbooks
{
    public partial class ValidateExperience : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;

        private string DefaultPanel => Request["panel"];

        private Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

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

            NextButton1.Click += NextButton1_Click;
            NextButton2.Click += NextButton2_Click;
            NextButton3.Click += NextButton3_Click;

            ValidateButton.Click += ValidateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && !HttpRequestHelper.IsAjaxRequest)
                LoadData();
        }

        private void NextButton1_Click(object sender, EventArgs e)
        {
            if (LoadCompetencies())
                CompetenciesPanel.IsSelected = true;
            else
            {
                LoadComments();
                CommentsPanel.IsSelected = true;
            }
        }

        private void NextButton2_Click(object sender, EventArgs e)
        {
            LoadComments();
            CommentsPanel.IsSelected = true;
        }

        private void NextButton3_Click(object sender, EventArgs e)
        {
            FinalPanel.Visible = true;
            FinalPanel.IsSelected = true;
        }

        private void ValidateButton_Click(object sender, EventArgs e)
        {
            var skillRating = SkillRating.ValueAsInt;

            ServiceLocator.SendCommand(new ValidateExperienceCommand(JournalIdentifier, ExperienceIdentifier, User.UserIdentifier, DateTimeOffset.UtcNow, skillRating));

            var url = $"/ui/admin/records/logbooks/outline-journal?journalsetup={JournalSetupIdentifier}&user={UserIdentifier}";

            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier, x => x.Journal.JournalSetup, x => x.Journal.JournalSetup.Event);
            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search");
                return;
            }

            var journalSetup = experience.Journal.JournalSetup;
            var content = ServiceLocator.ContentSearch.GetBlock(journalSetup.JournalSetupIdentifier, MultilingualString.DefaultLanguage);

            JournalIdentifier = experience.JournalIdentifier;
            JournalSetupIdentifier = journalSetup.JournalSetupIdentifier;
            UserIdentifier = experience.Journal.UserIdentifier;

            PageHelper.AutoBindHeader(this, null, content?.Title?.Text.Default ?? journalSetup.JournalSetupName);

            Detail.LoadData(journalSetup.JournalSetupIdentifier, experience.Journal.UserIdentifier, experience, false);

            SkillRating.Items.Clear();
            SkillRating.Items.Add(new ComboBoxOption());
            for (int i = 1; i <= 5; i++)
                SkillRating.Items.Add(new ComboBoxOption(i.ToString(), i.ToString()));

            SkillRating.ValueAsInt = experience.SkillRating;

            if (DefaultPanel == "competencies")
            {
                LoadCompetencies();

                CompetenciesPanel.IsSelected = true;
            }
            else if (DefaultPanel == "comments")
            {
                LoadCompetencies();
                LoadComments();

                CommentsPanel.IsSelected = true;
            }

            var returnUrl = GetReturnUrl();
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = $"/ui/admin/records/logbooks/outline-journal?journalsetup={journalSetup.JournalSetupIdentifier}&user={experience.Journal.UserIdentifier}";

            CancelButton1.NavigateUrl = returnUrl;
            CancelButton2.NavigateUrl = returnUrl;
            CancelButton3.NavigateUrl = returnUrl;
            FinalCancelButton.NavigateUrl = returnUrl;
        }

        private bool LoadCompetencies()
        {
            var hasCompetencies = Competencies.LoadData(ExperienceIdentifier, JournalSetupIdentifier, false);
            CompetenciesPanel.Visible = hasCompetencies;
            return hasCompetencies;
        }

        private void LoadComments()
        {
            var journal = ServiceLocator.JournalSearch.GetJournal(JournalIdentifier, x => x.Experiences);
            var experiences = journal.Experiences.ToList();

            Comments.LoadData(journal.JournalSetupIdentifier, journal.UserIdentifier, journal.JournalIdentifier, experiences, ExperienceIdentifier);
            CommentsPanel.Visible = true;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/outline"))
                return $"journalsetup={JournalSetupIdentifier}";

            return parent.Name.EndsWith("/outline-journal")
                ? $"journalsetup={JournalSetupIdentifier}&user={UserIdentifier}"
                : GetParentLinkParameters(parent, null);
        }
    }
}
