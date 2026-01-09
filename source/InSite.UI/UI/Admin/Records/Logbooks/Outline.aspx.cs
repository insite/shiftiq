using System;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Records.Logbooks
{
    public partial class Outline : AdminBasePage
    {
        protected Guid JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalsetup"], out var journalSetupIdentifier) ? journalSetupIdentifier : Guid.Empty;

        private string DefaultPanel => Request.QueryString["panel"];

        private Guid? AchievementIdentifier
        {
            get => (Guid?)ViewState[nameof(AchievementIdentifier)];
            set => ViewState[nameof(AchievementIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LockUnlockLogbookButton.Click += LockUnlockLogbookButton_Click;

            SearchInput.Click += SearchInput_Click;
            Groups.Refreshed += Groups_Refreshed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();

                if (DefaultPanel == "fields")
                    FieldsPanel.IsSelected = true;
                else if (DefaultPanel == "competencies")
                    CompetenciesPanel.IsSelected = true;
                else if (DefaultPanel == "users")
                    UsersPanel.IsSelected = true;
                else if (DefaultPanel == "setup")
                    LogbookPanel.IsSelected = true;
                else if (DefaultPanel == "achievements")
                    AchievementsPanel.IsSelected = true;
            }
        }

        private void LockUnlockLogbookButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);

            if (journalSetup == null)
                return;

            if (journalSetup.JournalSetupLocked.HasValue)
                ServiceLocator.SendCommand(new ChangeLockUnlockJournalSetup(journalSetup.JournalSetupIdentifier, null));
            else
                ServiceLocator.SendCommand(new ChangeLockUnlockJournalSetup(journalSetup.JournalSetupIdentifier, DateTimeOffset.Now));

            var url = $"/ui/admin/records/logbooks/outline?journalsetup={journalSetup.JournalSetupIdentifier}&panel=setup";

            HttpResponseHelper.Redirect(url);
        }

        private void SearchInput_Click(object sender, EventArgs e)
        {
            BindPeople();
            BindGroups();
        }

        private void Groups_Refreshed(object sender, EventArgs e)
        {
            BindGroups();
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search");
                return;
            }

            var validatorMessage = journalSetup.ValidatorMessageIdentifier.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(journalSetup.ValidatorMessageIdentifier.Value)
                : null;

            var learnerMessage = journalSetup.LearnerMessageIdentifier.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(journalSetup.LearnerMessageIdentifier.Value)
                : null;

            var learnerAddedMessage = journalSetup.LearnerAddedMessageIdentifier.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(journalSetup.LearnerAddedMessageIdentifier.Value)
                : null;

            PageHelper.AutoBindHeader(Page, qualifier: journalSetup.JournalSetupName);

            JournalSetupIdentifierLiteral.Text = JournalSetupIdentifier.ToString();

            JournalSetupName.Text = journalSetup.JournalSetupName;
            ClassTitle.Text = !string.IsNullOrEmpty(journalSetup.Event?.EventTitle)
                ? $"<a href=\"/ui/admin/events/classes/outline?event={journalSetup.Event?.EventIdentifier}\">{journalSetup.Event?.EventTitle} </a>"
                : "None";

            if (journalSetup.Event != null)
                ClassScheduled.Text = $"Scheduled: {GetLocalTime(journalSetup.Event.EventScheduledStart)} - {GetLocalTime(journalSetup.Event.EventScheduledEnd)}";

            AchievementTitle.Text = !string.IsNullOrEmpty(journalSetup.Achievement?.AchievementTitle)
                ? $"<a href=\"/ui/admin/records/achievements/outline?id={journalSetup.Achievement?.AchievementIdentifier}\">{journalSetup.Achievement?.AchievementTitle} </a>"
                : "None";

            FrameworkTitle.Text = !string.IsNullOrEmpty(journalSetup.Framework?.FrameworkTitle)
                ? $"<a href=\"/ui/admin/standards/edit?id={journalSetup.FrameworkStandardIdentifier}\">{journalSetup.Framework.FrameworkTitle} </a>"
                : "None";

            bool isDownloadable = journalSetup.AllowLogbookDownload.HasValue && journalSetup.AllowLogbookDownload.Value;

            IsValidationRequired.Text = journalSetup.IsValidationRequired ? "Yes" : "No";
            IsDownloadable.Text = isDownloadable ? "Yes" : "No";

            ValidatorMessageName.Text = !string.IsNullOrEmpty(validatorMessage?.MessageName)
                ? $"<a href=\"/ui/admin/messages/outline?message={validatorMessage.MessageIdentifier}\">{validatorMessage.MessageName} </a>"
                : "None";

            LearnerMessageName.Text = !string.IsNullOrEmpty(learnerMessage?.MessageName)
                ? $"<a href=\"/ui/admin/messages/outline?message={learnerMessage.MessageIdentifier}\">{learnerMessage.MessageName} </a>"
                : "None";

            LearnerAddedMessageName.Text = !string.IsNullOrEmpty(learnerAddedMessage?.MessageName)
                ? $"<a href=\"/ui/admin/messages/outline?message={learnerAddedMessage.MessageIdentifier}\">{learnerAddedMessage.MessageName} </a>"
                : "None";

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default;
            var instructions = content?[JournalSetupState.ContentLabels.Instructions].Text.Default;

            TitleOutput.Text = !string.IsNullOrEmpty(title) ? title : "None";
            Instructions.Text = !string.IsNullOrEmpty(instructions) ? Markdown.ToHtml(instructions) : "None";

            Fields.LoadData(JournalSetupIdentifier);

            CompetenciesPanel.Visible = journalSetup.Framework != null;

            if (journalSetup.Framework != null)
            {
                Competencies.LoadData(JournalSetupIdentifier, journalSetup.FrameworkStandardIdentifier.Value);
            }

            AchievementIdentifier = journalSetup.AchievementIdentifier;

            BindPeople();
            BindGroups();

            BindValidators();

            SetNavigationLinks();

            BindAchievements(journalSetup);

            BindLockUnlockView(journalSetup.JournalSetupLocked);
        }

        private void BindPeople()
        {
            var userLoadResult = Users.LoadData(JournalSetupIdentifier, AchievementIdentifier.HasValue, SearchInput.Text);
            PeoplePanel.Visible = userLoadResult.HasRows;

            AddUsers.NavigateUrl = $"/ui/admin/records/logbooks/add-users?journalsetup={JournalSetupIdentifier}";

            GrantButton.Visible = AchievementIdentifier.HasValue && userLoadResult.HasRows;
            GrantButton.NavigateUrl = $"/ui/admin/records/logbooks/achievements/grant-achievements?journalsetup={JournalSetupIdentifier}";

            ChangeFramework.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
            ChangeFramework.Visible = !userLoadResult.HasExperiences;
        }

        private void BindGroups()
        {
            var hasGroups = Groups.LoadData(JournalSetupIdentifier, SearchInput.Text);
            GroupPanel.Visible = hasGroups;
        }

        private void BindLockUnlockView(DateTimeOffset? journalSetupLocked)
        {
            if (journalSetupLocked == null)
                return;

            LockUnlockLogbookButton.Icon = "fas fa-lock-open";
            LockUnlockLogbookButton.Text = "Unlock";
            LogbookPanel.Title = "Logbook Template <i class=\"text-danger fas fa-lock ps-2\"></i>";
        }

        private void BindAchievements(QJournalSetup journalSetup)
        {
            var achievementSet = journalSetup.AchievementIdentifier.HasValue;

            AchievementsPanel.Visible = achievementSet;

            if (!achievementSet)
                return;

            var hasAchievements = Achievements.LoadData(JournalSetupIdentifier) > 0;

            AchievementsInnerPanel.Visible = hasAchievements;

            if (!hasAchievements)
                NoAchievements.AddMessage(AlertType.Warning, "No Achievements");
        }

        private void BindValidators()
        {
            var instructors = ServiceLocator.JournalSearch
                .GetJournalSetupUsers(new VJournalSetupUserFilter
                {
                    JournalSetupIdentifier = JournalSetupIdentifier,
                    Role = JournalSetupUserRole.Validator
                });

            ValidatorRepeater.DataSource = instructors;
            ValidatorRepeater.DataBind();

            AssignValidators.NavigateUrl = $"/ui/admin/records/logbooks/validators/assign?journalsetup={JournalSetupIdentifier}";
        }

        private void SetNavigationLinks()
        {
            DeleteLink.NavigateUrl = $"/ui/admin/records/logbooks/delete?journalsetup={JournalSetupIdentifier}";
            Rename.NavigateUrl = $"/ui/admin/records/logbooks/rename?journalsetup={JournalSetupIdentifier}";
            ChangeClass.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
            ChangeAchievement.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";

            ChangeIsValidationRequired.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
            ChangeIsRecordDownloadable.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(JournalSetupIdentifier, $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}");

            ChangeTitle.NavigateUrl = $"/ui/admin/records/logbooks/content?journalsetup={JournalSetupIdentifier}&tab={JournalSetupState.ContentLabels.Title}";
            ChangeInstructions.NavigateUrl = $"/ui/admin/records/logbooks/content?journalsetup={JournalSetupIdentifier}&tab={JournalSetupState.ContentLabels.Instructions}";

            ChangeValidatorMessage.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
            ChangeLearnerMessage.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
            ChangeLearnerAddedMessage.NavigateUrl = $"/ui/admin/records/logbooks/change?journalsetup={JournalSetupIdentifier}";
        }

        private static string GetLocalTime(DateTimeOffset? item)
            => item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
    }
}
