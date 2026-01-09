using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Admin.Courses.Courses;
using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common;
using InSite.Common.Web;
using InSite.Domain.CourseObjects;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Assessments.Attempts.Utilities;
using InSite.UI.Portal.Integrations.Scorm;
using InSite.UI.Portal.Learning.Controls;
using InSite.UI.Portal.Learning.Models;
using InSite.Web.Integration;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Learning
{
    public partial class Course : PortalBasePage
    {
        #region Properties

        private const string BlankHref = "about:blank";

        public override string ActionUrl => "ui/portal/learning/course";

        protected string TermsData
        {
            get => (string)ViewState[nameof(TermsData)];
            set => ViewState[nameof(TermsData)] = value;
        }

        public bool AreModulesUnlocked
        {
            get => Session[nameof(AreModulesUnlocked)] is bool value && value;
            set => Session[nameof(AreModulesUnlocked)] = value;
        }

        public bool AreActivitiesUnlocked
        {
            get => Session[nameof(AreActivitiesUnlocked)] is bool value && value;
            set => Session[nameof(AreActivitiesUnlocked)] = value;
        }

        protected Domain.CourseObjects.ProgressModel Progress => _state.Model;
        protected Persistence.Content.PortalPageModel PortalPage => _state.PortalPage;

        protected int SidebarWidth
        {
            get => (int)(ViewState[nameof(SidebarWidth)] ?? 105);
            set => ViewState[nameof(SidebarWidth)] = value;
        }

        #endregion

        #region Fields

        private GlossaryHelper _glossaryHelper;
        private ScormIntegrator _scormLauncher;
        private ProgressState _state;

        private bool? _hasExpiredItems;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DoneButton.Click += DoneButton_Click;
            StartButton.Click += NextButton_Click;
            NextButton.Click += NextButton_Click;
            PrevButton.Click += PrevButton_Click;

            PostCommentButton.Click += PostCommentButton_Click;
            RestartCourseButton.Click += RestartCourseButton_Click;
            UnlockModulesButton.Click += UnlockModulesButton_Click;
            UnlockActivitiesButton.Click += UnlockActivitiesButton_Click;

            StartAssessmentLink.Click += StartAssessmentLink_Click;
            ViewAssessmentLink.Click += ViewAssessmentLink_Click;
        }

        private void PostCommentButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PostCommentInput.CommentTextValue))
                return;

            var course = Progress.Course.Identifier;

            var reply = new QComment
            {
                AuthorUserIdentifier = User.Identifier,
                CommentIdentifier = UniqueIdentifier.Create(),
                CommentText = PostCommentInput.CommentTextValue,
                ContainerIdentifier = course,
                ContainerSubtype = "Discussion",
                ContainerType = "Course",
                OrganizationIdentifier = Organization.Identifier,
                TimestampCreated = DateTimeOffset.Now
            };
            QCommentStore.Insert(reply);
            PostCommentRepeater.LoadData(course);
            PostCommentInput.Clear();
        }

        private void StartAssessmentLink_Click(object sender, EventArgs e)
        {
            var form = Progress.CurrentActivity?.Assessment?.Identifier ?? Guid.Empty;
            if (form == Guid.Empty)
                return;

            var autoStart = IsAssessmentAutoStartRequired(Organization)
                || IsAssessmentAutoStartRequested(StartAssessmentLink.CommandName);

            if (autoStart)
            {
                var attempts = ServiceLocator.AttemptSearch.GetAttempts(new QAttemptFilter { LearnerUserIdentifier = User.UserIdentifier, FormIdentifier = form });
                foreach (var attempt in attempts)
                    ServiceLocator.SendCommand(new VoidAttempt(attempt.AttemptIdentifier, "Learner Restarted"));
            }

            var url = AttemptUrlForm.GetStartUrl(form, autoStart);
            HttpResponseHelper.Redirect(url);
        }

        private bool IsAssessmentAutoStartRequired(Domain.Organizations.OrganizationState organization)
        {
            return ServiceLocator.Partition.IsE03()
                || organization.Toolkits.Assessments.RequireAutoStart;
        }

        private bool IsAssessmentAutoStartRequested(string command)
        {
            return StringHelper.EqualsAny(command, new[] { "ForceRestart", "Restart" });
        }

        private void ViewAssessmentLink_Click(object sender, EventArgs e)
        {
            var form = Progress.CurrentActivity?.Assessment?.Identifier ?? Guid.Empty;
            if (form == Guid.Empty)
                return;

            var attempt = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(form, User.Identifier)?.AttemptLastSubmittedIdentifier;
            if (attempt == null)
                return;

            var urlBase = AttemptUrlForm.Create(form, attempt.Value);
            var url = urlBase.GetResultUrl(attempt.Value);
            HttpResponseHelper.Redirect(url);
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!LoadState())
                return;

            if (IsPostBack)
                return;

            PageHelper.HideBreadcrumbs(this);

            CheckCurrentActivityLocked();

            var model = Progress;

            _glossaryHelper = new GlossaryHelper(Identity.Language);
            TermsData = _glossaryHelper.GetJavaScriptDictionary().IfNullOrEmpty("null");

            ValidAlert.Text = LabelHelper.GetTranslation("[Portal.Learning.Course].[ValidAlertText]", Identity.Language, isMarkdown: true);
            ExpiredAlert.Text = LabelHelper.GetTranslation("[Portal.Learning.Course].[ExpiredAlertText]", Identity.Language, isMarkdown: true);
            AlertRestartInfo.Text = LabelHelper.GetTranslation("[Portal.Learning.Course].[AlertRestartInfoText]", Identity.Language, isMarkdown: true);

            if (Page.Master is PortalMaster m)
            {
                m.HideBreadcrumbsAndTitle();
                SidebarWidth = (Progress.Course.OutlineWidth ?? 4) * 105;
            }

            if (model.Course.Gradebook?.IsLocked == true)
            {
                ControlButtons.Visible = false;
                OutlineList.Visible = false;
                ErrorAlert.AddMessage(AlertType.Error, $"{Translate("This course is locked. Please contact your Administrator")}.");
                return;
            }

            if ((PortalPage.Page.IsHidden && model.Course.Closed.HasValue) || (!PortalPage.Page.IsHidden && model.Course.Closed.HasValue && model.Course.Closed.Value <= DateTimeOffset.Now))
                ErrorAlert.AddMessage(AlertType.Error, $"{Translate($"This Course closed on {model.Course.Closed.Format(User.TimeZone, isHtml: true, nullValue: string.Empty)}")}.");

            AddBreadcrumb(Translate("Home"), GetHomeUrl(model.Course.Identifier));
            AddBreadcrumb(model.Course.Content.Title.GetText(CurrentLanguage), null);

            CourseStyles.Controls.Add(new Literal { Text = model.Course.Style });

            StartButton.Text = LabelSearch.GetTranslation("Courses.Overview.StartButton", CurrentLanguage, Organization.Identifier, false, true, "Start");

            LoadProgress();

            ScormSyncGradebook();

            LoadOverviewPanel();

            if (Organization.ParentOrganizationIdentifier == OrganizationIdentifiers.CMDS)
                AlertRestartInfo.Visible = true;

            if (!model.CurrentPageIsValid)
                RedirectToFirstActivity();

            LoadButtons(model);

            UnitRepeater.BindModelToControls(model, AreModulesUnlocked, AreActivitiesUnlocked);

            if (model.CurrentPage == 0)
                NextButton.Text = Translate("Start");
            else
                LoadCurrentPage();
        }

        private void LoadButtons(Domain.CourseObjects.ProgressModel model)
        {
            if (Identity.IsImpersonating)
            {
                AreModulesUnlocked = false;
                AreActivitiesUnlocked = false;
            }

            RestartCourseButton.OnClientClick = $"return confirm('Are you sure you want to restart this course? Your progress will be reset.');";
            RestartCourseButton.Visible = ServiceLocator.Partition.IsE03() || (Identity.IsAdministrator && !Identity.IsImpersonating) || _hasExpiredItems == true;

            UnlockActivitiesButton.Visible = Identity.IsAdministrator;
            UnlockModulesButton.Visible = Identity.IsAdministrator;

            UnlockActivitiesLiteral.Text = AreActivitiesUnlocked ? "All activities unlocked" : "Unlock all visible activities";
            UnlockModulesLiteral.Text = AreModulesUnlocked ? "All units and modules unlocked" : "Unlock all units and modules";

            UnlockModulesIcon.Attributes["class"] = AreModulesUnlocked
                ? "fas fa-unlock-keyhole me-1 text-success"
                : "fas fa-lock-keyhole me-1 text-danger";

            UnlockActivitiesIcon.Attributes["class"] = AreActivitiesUnlocked
                ? "fas fa-unlock-keyhole me-1 text-success"
                : "fas fa-lock-keyhole me-1 text-danger";

            PrevButton.Visible = model.CurrentPage > 1 && model.PreviousPage.HasValue;
            NextButton.Visible = model.CurrentPage == 0
                ? !OverviewAboutPanel.Visible && !OverviewDiscussionPanel.Visible && !OverviewAchievementPanel.Visible
                : model.CurrentPage < model.GetVisibleActivities().Count && model.NextPage.HasValue;
        }

        private bool LoadState()
        {
            if (_state != null)
                return true;

            _state = new ProgressState(RouteData, Request.QueryString);

            if (!_state.LoadModel())
                HttpResponseHelper.Redirect(GetHomeUrl(Guid.Empty), true);

            if (Progress.Course == null)
            {
                ShowCriticalError(GetDisplayText("No course is specified for this URL."));
                return false;
            }

            _state.LoadModelProgress();

            if (Progress.IsCourseHidden)
            {
                ShowCriticalError("Your account is not granted permission to access this course.");
                return false;
            }

            return true;
        }

        private void ShowCriticalError(string error)
        {
            if (Page.Master is PortalMaster m)
            {
                m.HideBreadcrumbsAndTitle();
                m.SidebarVisible(false);
            }

            CourseStyles.Visible = false;
            EnabledPanel.Visible = false;
            TermWindow.Visible = false;

            CriticalAlert.AddMessage(AlertType.Error, error);
        }

        private void LoadProgress()
        {
            ProgramHelper.EnrollLearnerByObjectId(Organization.Identifier, User.Identifier, Progress.Course.Identifier);

            CheckNotCompletedCourse();

            var model = Progress;
            var gradebook = model.Course.Gradebook;
            var activity = model.CurrentActivity;

            if (gradebook == null)
                return;

            if (gradebook.IsLocked)
            {
                ServiceLocator.SendCommand(new UnlockGradebook(gradebook.Identifier));
                gradebook.IsLocked = false;
            }

            if (model.CurrentPage <= 0 || activity.GradeItem == null)
                return;

            RecordProgress(gradebook.Identifier, activity.GradeItem.Identifier, Start);

            if (activity.Requirement == RequirementType.View)
            {
                if (!model.CurrentActivityResult.Completed.HasValue)
                {
                    RecordProgress(gradebook.Identifier, activity.GradeItem.Identifier, Complete);
                    Response.Redirect(Request.RawUrl);
                }
            }
            else if (activity.Requirement == RequirementType.CompleteScorm)
            {
                if (!model.CurrentActivityResult.Completed.HasValue)
                {
                    var launcher = GetScormCloud();

                    if (ScormContentIsCompleted(launcher.ActivityHook, User.UserIdentifier))
                    {
                        RecordProgress(gradebook.Identifier, activity.GradeItem.Identifier, Complete);

                        _state.Reload();
                    }
                }
            }

            _state.LoadModelProgress(Progress.CurrentActivity.Identifier, Progress.CurrentActivity.GradeItem.Identifier);
        }

        private void CheckNotCompletedCourse()
        {
            var activityId = Progress.Course.CompletionActivityIdentifier;

            if (activityId == null)
                return;

            if (!ServiceLocator.CourseObjectSearch.IsActivityCompleted(activityId.Value, User.Identifier))
                ServiceLocator.CourseObjectStore.InsertCourseUser(Progress.Course.Identifier, User.Identifier);
        }

        private void CheckCurrentActivityLocked()
        {
            var learningResult = Progress.CurrentActivityResult;
            if (learningResult.IsLocked && !AreActivitiesUnlocked)
                HttpResponseHelper.Redirect(Progress.CourseUrl);
        }

        private void LoadOverviewPanel()
        {
            var path = _state.PortalPage.Path;
            CourseOverviewLink.HRef = HttpResponseHelper.BuildUrl(path, "tab=overview");
            CourseDiscussionsLink.HRef = HttpResponseHelper.BuildUrl(path, "tab=discussions");
            CourseAchievementsLink.HRef = HttpResponseHelper.BuildUrl(path, "tab=achievements");

            var gradebook = Progress.Course.Gradebook;
            var hasAchievements = false;

            if (gradebook != null)
            {
                if (ServiceLocator.Partition.IsE03())
                {
                    if (gradebook.Achievement != null)
                    {
                        var achievement = ServiceLocator.AchievementSearch.GetAchievement(gradebook.Achievement.Value);
                        hasAchievements = (achievement?.CertificateLayoutCode).IsNotEmpty();
                    }

                    var gradeitem = gradebook.Items.FirstOrDefault(x => x.Achievement.HasValue);

                    if (!hasAchievements && gradeitem?.Achievement != null)
                    {
                        var achievement = ServiceLocator.AchievementSearch.GetAchievement(gradeitem.Achievement.Value);
                        hasAchievements = (achievement?.CertificateLayoutCode).IsNotEmpty();
                    }
                }
                else
                    hasAchievements = gradebook.Items.Any(x => x.Achievement != null);
            }

            CourseDiscussionsLink.Visible = Progress.Course.AllowDiscussion;
            CourseAchievementsLink.Visible = hasAchievements;

            if (Progress.CurrentPage == 0)
                LoadOverviewAchievements();
        }

        private void LoadOverviewAchievements()
        {
            var tabValue = Page.Request.QueryString["tab"];
            var isOverviewTab = tabValue == "overview";
            var isDiscussionsTab = tabValue == "discussions";
            var isAchievementsTab = tabValue == "achievements";

            OverviewPanel.Visible = true;

            if (isDiscussionsTab)
                LoadDiscussionsPanel();
            else if (isAchievementsTab)
                LoadAchievementsPanel();
            else
                LoadOverviewAboutPanel();

            ValidAlert.Visible = false;
            ExpiredAlert.Visible = false;

            if (Progress.Course.Gradebook == null)
                return;

            ProgressSummaryLink.Text = "<i class='far fa-file-check me-2'></i>" + Translate("Progress Report and Scores");
            ProgressSummaryLink.NavigateUrl = $"/ui/portal/learning/progress?course={Progress.Course.Identifier}";
            ReportsPanel.Visible = Progress.Course.IsProgressReportEnabled;

            var items = GetCertificateRepeaterItems(Progress.Course.Gradebook.Items);
            if (items.Length == 0)
                return;

            if (!isOverviewTab && !isDiscussionsTab && items.All(x => x.CredentialGranted.HasValue))
                LoadAchievementsPanel();

            CertificateRepeater.BindModelToControl(items);

            var hasValidItems = items.All(x => x.CredentialStatus == "Valid");
            var hasExpiredItems = items.All(x => x.CredentialStatus == "Expired");

            ValidAlert.Visible = hasValidItems
                && items.Any(x => x.HasBadgeImage == true && x.BadgeUrl.IsNotEmpty()
                                || x.CredentialStatus.ToEnumNullable<CredentialStatus>() == CredentialStatus.Valid
                                    && (ServiceLocator.Partition.IsE03() || x.CertificationLayoutCode.IsNotEmpty()));
            ExpiredAlert.Visible = hasExpiredItems;
            AlertRestartInfo.Visible = hasExpiredItems;

            _hasExpiredItems = hasExpiredItems;
        }

        private static CertificateRepeaterItem[] GetCertificateRepeaterItems(IEnumerable<GradeItem> items)
        {
            var ids = items
                .Where(x => x.Achievement != null).Select(x => x.Achievement.Value)
                .Distinct().ToList();
            var achievements = ServiceLocator.AchievementSearch.GetAchievements(ids)
                .ToDictionary(x => x.AchievementIdentifier, x => x);
            var credentials = ServiceLocator.AchievementSearch.GetCredentials(ids, User.UserIdentifier)
                .ToDictionary(x => x.AchievementIdentifier, x => x); ;

            var result = new List<CertificateRepeaterItem>();

            foreach (var id in ids)
            {
                if (!achievements.TryGetValue(id, out var a))
                    continue;

                var item = new CertificateRepeaterItem
                {
                    AchievementTitle = a.AchievementTitle,
                    AchievementLabel = a.AchievementLabel,
                    CertificationLayoutCode = a.CertificateLayoutCode,
                    HasBadgeImage = a.HasBadgeImage,
                    BadgeUrl = a.BadgeImageUrl,
                    AchievementIdentifier = a.AchievementIdentifier,
                    CredentialStatus = "Pending"
                };

                if (credentials.TryGetValue(id, out var c))
                {
                    item.CredentialIdentifier = c.CredentialIdentifier;
                    item.CredentialGranted = c.CredentialGranted;
                    item.CredentialExpirationExpected = c.CredentialExpirationExpected;

                    if (c.CredentialStatus.IsNotEmpty())
                        item.CredentialStatus = c.CredentialStatus;
                }

                result.Add(item);
            }

            return result.ToArray();
        }

        private void LoadOverviewAboutPanel()
        {
            CourseOverviewLink.Attributes["class"] = "active";

            var content = ServiceLocator.ContentSearch.GetBlock(Progress.Course.Identifier, CurrentLanguage);
            var body = content.Body.GetHtml(CurrentLanguage);
            OverviewAboutText.InnerHtml = body;
            OverviewAboutPanel.Visible = !string.IsNullOrWhiteSpace(body);
            NextButton.Visible = true;
        }

        private void LoadDiscussionsPanel()
        {
            CourseDiscussionsLink.Attributes["class"] = "active";
            CourseAchievementsLink.Attributes["class"] = "";
            CourseOverviewLink.Attributes["class"] = "";

            OverviewAboutPanel.Visible = false;
            OverviewDiscussionPanel.Visible = true;
            OverviewAchievementPanel.Visible = false;
            NextButton.Visible = false;

            PostCommentRepeater.LoadData(Progress.Course.Identifier);
        }

        private void LoadAchievementsPanel()
        {
            CourseDiscussionsLink.Attributes["class"] = "";
            CourseAchievementsLink.Attributes["class"] = "active";
            CourseOverviewLink.Attributes["class"] = "";

            OverviewAboutPanel.Visible = false;
            OverviewDiscussionPanel.Visible = false;
            OverviewAchievementPanel.Visible = CourseAchievementsLink.Visible;
        }

        #endregion

        #region Loading (current page)

        private void LoadCurrentPage()
        {
            var model = Progress;

            LoadDonePanel();

            LoadCourseBreadcrumbs();

            AddBreadcrumb($"{Translate("Activity")} {model.CurrentPage} {Translate("of")} {model.GetVisibleActivities().Count}", null);

            // If it's an incomplete assessment or form then hide the Next button.
            if (StringHelper.Equals(model.CurrentActivity.Type, "Survey") && !model.CurrentActivityResult.IsCompleted
                || StringHelper.Equals(model.CurrentActivity.Type, "Assessment") && !model.CurrentActivityResult.IsCompletedOrFailed
                )
            {
                NextButton.Visible = false;
            }

            var aSummary = model.CurrentActivity.Content.Summary.GetHtml(CurrentLanguage, true);

            ActivitySummary.Visible = aSummary.IsNotEmpty();
            ActivitySummary.InnerHtml = _glossaryHelper.Process(
                model.CurrentActivity.Identifier,
                ContentLabel.Body,
                aSummary);

            if ((model.CurrentActivityResult.IsLocked || model.CurrentActivityResult.IsHidden || model.CurrentModuleResult.IsLocked || model.CurrentModuleResult.IsHidden) && !(AreModulesUnlocked || AreActivitiesUnlocked))
                LoadNoAccessPanel();
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Assessment"))
            {
                if (model.CurrentActivity.Assessment != null)
                    LoadAssessmentPanel();
            }
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Document"))
                LoadDocumentPanel();
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Lesson"))
                LoadLessonPanel();
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Link"))
                LoadLinkPanel();
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Survey"))
                LoadSurveyPanel();
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Video"))
                LoadVideoPanel();
            else if (StringHelper.Equals(model.CurrentActivity.Type, "Quiz"))
                LoadQuizPanel();
            else
                throw new NotSupportedException($"Activity type is not supported: {model.CurrentActivity.Type}");
        }

        private void LoadCourseBreadcrumbs()
        {
            var model = Progress;
            var courseUrl = RoutingConfiguration.PortalCourseUrl(Progress.Course.Identifier);
            var courseTitle = Progress.Course.Content.Title.GetText(CurrentLanguage, true);
            var unitTitle = model.CurrentActivity.Module.Unit.Content.Title.GetText(CurrentLanguage, true);
            var moduleTitle = model.CurrentActivity.Module.Content.Title.GetText(CurrentLanguage, true);

            var unitTitleVisible = Progress.Course.AllowMultipleUnits && !StringHelper.Equals(courseTitle, unitTitle) && !StringHelper.Equals(courseTitle, moduleTitle);

            var html = new StringBuilder();
            html.Append("<nav aria-label='breadcrumb'><ol class='pt-1 mt-2 breadcrumb'>");
            html.Append($"<li class='breadcrumb-item'><a href='{courseUrl}'>{courseTitle}</a></li>");
            if (unitTitleVisible)
                html.Append($"<li class='breadcrumb-item active'>{unitTitle}</li>");
            html.Append($"<li class='breadcrumb-item active'>{moduleTitle}</li>");
            html.Append("</ol></nav>");

            CourseBreadcrumbs.Text = html.ToString();
        }

        private void LoadDonePanel()
        {
            DonePanel.Visible = Progress.Course.Gradebook != null && Progress.CurrentActivity.GradeItem != null
                && Progress.CurrentActivity.Requirement == RequirementType.MarkAsDone;

            var done = Progress.CurrentActivityResult.IsCompleted;
            var completed = Progress.CurrentActivityResult.Completed;

            DoneContent.Visible = done;
            NotDoneContent.Visible = !done;

            DoneButton.Text = Progress.CurrentActivity.DoneButtonText.IfNullOrEmpty(QActivity.DefaultDoneButtonText);
            DoneButtonInstructions.Text = Markdown.ToHtml(Progress.CurrentActivity.DoneButtonInstructions.IfNullOrEmpty(QActivity.DefaultDoneButtonInstructions), true);
            DoneMarkedInstructions.Text = Markdown.ToHtml(Progress.CurrentActivity.DoneMarkedInstructions.IfNullOrEmpty(QActivity.DefaultDoneMarkedInstructions), true);
            DoneMarkedInstructions.Arguments = done && completed.HasValue
                ? new[] { TimeZones.Format(completed.Value, User.TimeZone) }
                : null;
        }

        private void LoadNoAccessPanel()
        {
            NoAccessPanel.Visible = true;
            NextButton.Visible = false;
            DoneButton.Visible = false;
            StartButton.Visible = false;
        }

        private void LoadAssessmentPanel()
        {
            var model = Progress;
            var activity = model.CurrentActivity;
            var assessment = activity.Assessment;

            var form = assessment != null
                ? ServiceLocator.BankSearch.GetForm(assessment.Identifier)
                : null;
            var hasForm = form != null;

            AssessmentPanel.Visible = hasForm;

            if (!hasForm)
                return;

            var result = model.CurrentActivityResult;
            var attemptSummary = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(form.FormIdentifier, model.User)
                ?? new TLearnerAttemptSummary();

            if (ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Production)
                AttemptMetadata.InnerHtml = $"<i class='far fa-info-circle'></i> Attempt Limit = {form.FormAttemptLimit}. Attempt Count = {attemptSummary.AttemptSubmittedCount}.";

            GradeLabel.Visible = false;
            StartAssessmentLink.Visible = false;
            ViewAssessmentLink.Visible = false;

            var hasNoAttempt = attemptSummary.AttemptStartedCount == 0;
            var hasActiveAttempt = attemptSummary.AttemptLastStartedIdentifier.HasValue
                && attemptSummary.AttemptLastStartedIdentifier != attemptSummary.AttemptLastSubmittedIdentifier;

            if (hasNoAttempt || hasActiveAttempt)
            {
                var body = activity.Content.Body.GetHtml(CurrentLanguage, true);
                if (!string.IsNullOrWhiteSpace(body))
                    AssessmentBody.InnerHtml = _glossaryHelper.Process(activity.Identifier, ContentLabel.Body, body);
                else
                    AssessmentBody.InnerHtml = $"<h2>{activity.Content.Title.Text[CurrentLanguage]}</h2>Please assess your knowledge with this quiz.";

                if (hasActiveAttempt)
                    SetAssessmentLink(StartAssessmentLink, "Resume", "play", "Resume");
                else if (hasNoAttempt)
                    SetAssessmentLink(StartAssessmentLink, "Start", "rocket", "Start");

                return;
            }
            else
            {
                SetRestartButton();
            }

            AssessmentBody.InnerHtml = "<div class='alert alert-success' role='alert'>Your assessment submission is completed.</div>";

            var disclosure = activity.Assessment.Disclosure.ToEnum(DisclosureType.None);

            if (disclosure == DisclosureType.Score || disclosure == DisclosureType.Full)
            {
                if (result.IsPassed)
                    SetGradeLabel("Passed", "check", "success");
                else if (result.IsFailed)
                    SetGradeLabel("Failed", "times", "danger");
                else
                    SetGradeLabel("Completed", "question-circle", null);
            }

            if (disclosure != DisclosureType.None)
                SetAssessmentLink(ViewAssessmentLink, "View", "search", "View");

            void SetAssessmentLink(LinkButton link, string text, string icon, string command)
            {
                text = Translate(text);

                link.CommandName = command;
                link.Text = $"<i class='far fa-{icon} me-1'></i> {text}";
                link.Visible = true;
            }

            void SetGradeLabel(string text, string icon, string category)
            {
                var labelClass = "fw-bold";
                if (category.IsNotEmpty())
                    labelClass += $" text-{category}";

                GradeLabel.Visible = true;
                GradeLabel.Attributes["class"] = labelClass;

                GradeLabel.InnerHtml = $"<i class='far fa-{icon} me-1'></i> {Translate(text)} ";
                if (result.Completed.HasValue)
                    GradeLabel.InnerHtml += TimeZones.FormatDateOnly(result.Completed.Value, User.TimeZone);
            }

            void SetRestartButton()
            {
                var summary = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(activity.Assessment.Identifier, model.User);
                if (summary == null)
                    return;

                var allow = form.FormAttemptLimit == 0 || summary.AttemptStartedCount < form.FormAttemptLimit;
                var isAttemptedAtLeastOnce = summary.AttemptStartedCount > 0;
                var isLastAttempt = form.FormAttemptLimit > 0 && summary.AttemptStartedCount >= form.FormAttemptLimit;
                var hasIncompleteAttempt = summary.AttemptStartedCount > summary.AttemptSubmittedCount;

                if (allow && isAttemptedAtLeastOnce && !isLastAttempt && !hasIncompleteAttempt)
                {
                    SetAssessmentLink(StartAssessmentLink, "Restart", "redo", "ForceStart");
                }
            }
        }

        private void LoadDocumentPanel()
        {
            var model = Progress;

            var languageUrl = model.CurrentActivity.IsMultilingual
                ? OutlineHelper.GetLinkByLanguage(model.CurrentActivity.Link?.Url, CurrentLanguage)
                : model.CurrentActivity.Link?.Url;

            var embedHtml = OutlineHelper.GenerateEmbedDocument(languageUrl);
            var isEmbed = embedHtml.IsNotEmpty();

            DocumentPanel.Visible = true;
            DocumentDescription.InnerHtml = _glossaryHelper.Process(
                model.CurrentActivity.Identifier,
                ContentLabel.Body,
                model.CurrentActivity.Content.Body.GetHtml(CurrentLanguage, true));

            DocumentLaunchLink.Visible = !isEmbed;
            DocumentLaunchLink.HRef = languageUrl;
            DocumentLaunchLink.Target = model.CurrentActivity.Link?.Target;

            DocumentEmbedContent.Visible = isEmbed;
            DocumentEmbedContent.Text = embedHtml;
        }

        private void LoadLessonPanel()
        {
            var body = Progress.CurrentActivity.Content.Body.GetHtml(CurrentLanguage, true);
            var hasBody = body.IsNotEmpty();

            LessonPanel.Visible = hasBody;

            if (!hasBody)
                return;

            body = body
                .Replace("<img ", "<img class='figure-img img-thumbnail' ")
                .Replace("<blockquote>", "<blockquote class='blockquote'>")
                .Replace("<table class=\"table-markdown\">", "<table class='table table-striped'>")
                .Replace("<table>", "<table class='table table-striped'>")
                ;

            LessonPanel.InnerHtml = _glossaryHelper.Process(
                Progress.CurrentActivity.Identifier,
                ContentLabel.Body,
                body);
        }

        private void LoadLinkPanel()
        {
            var activity = Progress.CurrentActivity ?? throw new ArgumentNullException("_state.Model.CurrentActivity");
            var isScorm = activity.Link?.Type == "SCORM";

            LinkPanel.Visible = !isScorm;
            LinkPanelScorm.Visible = isScorm;

            if (isScorm)
            {
                LoadLinkPanelScorm();
                return;
            }

            var body = activity.Content?.Body ?? throw new ArgumentNullException("activity.Content");

            LinkDescription.InnerHtml = _glossaryHelper.Process(
                activity.Identifier,
                ContentLabel.Body,
                body.GetHtml(CurrentLanguage, true));

            var embedHtml = activity.Link?.Target == "_embed"
                ? OutlineHelper.GenerateEmbedLink(activity.Link.Url)
                : null;
            var isEmbed = embedHtml.IsNotEmpty();

            LinkLaunchLink.Visible = !isEmbed;
            LinkEmbedContent.Visible = isEmbed;

            if (isEmbed)
            {
                LinkEmbedContent.Text = embedHtml;
            }
            else
            {
                var link = activity.Link ?? throw new ArgumentNullException("activity.Link");
                var title = activity.Content?.Title ?? throw new ArgumentNullException("activity.Content.Title");

                LinkLaunchLink.HRef = link.Url;
                LinkLaunchLink.Target = link.Target;
                LinkLaunchLink.InnerText = title.GetText(CurrentLanguage, true);
            }
        }

        private void LoadLinkPanelScorm()
        {
            var activity = Progress.CurrentActivity;

            var link = activity.Link;

            var courseId = Progress.Course.Identifier;

            var activityId = activity.Identifier;

            ScormBody.InnerHtml = activity.Content.Body.GetHtml(CurrentLanguage, true);

            if (activity.DurationMinutes.HasValue)
                ScormTimeRequired.Text = Translator.Translate("Time Required")
                    + ": "
                    + TimeSpan.FromMinutes(activity.DurationMinutes.Value).Humanize(precision: 2, culture: CurrentCulture);

            if (link.Target == "_self" || link.Target == "_top" || link.Target == "_embed")
            {
                var href = $"/ui/portal/integrations/scorm/launch/{activity.Identifier}";

                if (activity.ContentDeliveryPlatform == "Scoop")
                {
                    var scormPackageSlug = activity.Hook;

                    // Scoop does not use the activity identifier as for a SCORM Package ID. Instead it uses a URL-
                    // friendly alphanumeric slug. Therefore, if the SCORM Package Slug is missing (or if it matches the
                    // Shift activity identifier) then the launch button should navigate to a blank page.

                    href = scormPackageSlug.IsEmpty() || StringHelper.Equals(scormPackageSlug, activityId.ToString())
                        ? BlankHref
                        : Launch.GetScoopLaunchUrl(scormPackageSlug, courseId, activityId);
                }

                ScormStartUrl.HRef = href;
                ScormStartUrl.Target = "_self";
            }
            else
            {
                ScormStartUrl.HRef = link.Url;
                ScormStartUrl.Target = link.Target;
            }

            // If the launch button has no valid URL then display the button in red with an informative error message,
            // and force it to navigate to a blank page. This is a user error; we do not need support requests or bug
            // reports or error logs in this scenario.

            if (IsEmptyHref(ScormStartUrl.HRef))
            {
                ScormStartUrl.Attributes["class"] = "btn btn-sm btn-danger";
                ScormStartUrl.HRef = BlankHref;
                ScormStartUrl.InnerHtml = $"<i class='fas fa-triangle-exclamation me-2'></i>Missing SCORM Package ID";
            }
        }

        private bool IsEmptyHref(string href)
        {
            return href.IsEmpty() || StringHelper.EqualsAny(ScormStartUrl.HRef, new[] { "#", BlankHref });
        }

        private void LoadQuizPanel()
        {
            var activity = Progress.CurrentActivity ?? throw new ArgumentNullException("_state.Model.CurrentActivity");

            QuizPanel.Visible = true;

            var body = activity.Content?.Body ?? throw new ArgumentNullException("activity.Content");

            QuizBody.InnerHtml = _glossaryHelper.Process(
                activity.Identifier,
                ContentLabel.Body,
                body.GetHtml(CurrentLanguage, true));

            QuizLaunch.HRef = $"/ui/portal/assessments/quiz-attempts/start?activity={activity.Identifier}";
        }

        private void LoadSurveyPanel()
        {
            SurveyPanel.Visible = true;

            var model = Progress;
            var activity = Progress.CurrentActivity;

            var body = activity.Content.Body.GetHtml(CurrentLanguage, true);
            if (!string.IsNullOrWhiteSpace(body))
                SurveyBody.InnerHtml = _glossaryHelper.Process(activity.Identifier, ContentLabel.Body, body);
            else
                SurveyBody.InnerHtml = $"<h2>{activity.Content.Title.Text[CurrentLanguage]}</h2>Please take the form.";

            var survey = activity.Survey != null
                ? ServiceLocator.SurveySearch.GetSurveyForm(activity.Survey.Identifier)
                : null;
            var isSurveyFound = survey != null;

            SurveyLink.Visible = isSurveyFound;

            if (!isSurveyFound)
                return;

            var lastSession = ServiceLocator.SurveySearch.GetResponseSession(new QResponseSessionFilter
            {
                SurveyFormIdentifier = activity.Survey.Identifier,
                RespondentUserIdentifier = User.UserIdentifier,
                OrderBy = "ResponseSessionStarted DESC"
            });

            if (model.CurrentActivityResult.IsCompleted && lastSession != null)
            {
                var linkText = Translate("Review Form");
                SurveyLink.InnerHtml = $"<i class='far fa-search me-1'></i> {linkText}";
                SurveyLink.HRef = $"/ui/portal/workflow/forms/submit/review?session={lastSession.ResponseSessionIdentifier}";
            }
            else if (model.CurrentActivityResult.IsStarted && lastSession != null)
            {
                var linkText = Translate("Resume Form");
                SurveyLink.InnerHtml = $"<i class='far fa-play me-1'></i> {linkText}";
                SurveyLink.HRef = $"/ui/portal/workflow/forms/submit/resume?session={lastSession.ResponseSessionIdentifier}";
            }
            else
            {
                var linkText = Translate("Start Form");
                SurveyLink.InnerHtml = $"<i class='far fa-rocket me-1'></i> {linkText}";
                SurveyLink.HRef = $"/form/{survey.AssetNumber}/{User.UserIdentifier}";
            }
        }

        private void LoadVideoPanel()
        {
            var activity = _state?.Model?.CurrentActivity ?? throw new ArgumentNullException("_state.Model.CurrentActivity");

            if (string.IsNullOrEmpty(activity.Link?.Url) || string.IsNullOrEmpty(activity.Link.Target))
            {
                ErrorAlert.AddMessage(AlertType.Error, $"{Translate("This video is not properly configured")}.");
                return;
            }

            var languageUrl = activity.IsMultilingual
                ? OutlineHelper.GetLinkByLanguage(activity.Link.Url, CurrentLanguage)
                : activity.Link.Url;

            VideoPanel.Visible = true;

            if (activity.Content?.Body != null)
            {
                var body = activity.Content.Body.GetHtml(CurrentLanguage, true);
                VideoDescription.InnerHtml = _glossaryHelper.Process(activity.Identifier, ContentLabel.Body, body);
            }

            var target = activity.Link.Target;

            var embedHtml = string.Equals(target, "_embed", StringComparison.OrdinalIgnoreCase)
                ? OutlineHelper.GenerateEmbedVideo(languageUrl, 840, 470)
                : null;

            var isEmbed = embedHtml.IsNotEmpty();

            VideoLaunchLink.Visible = !isEmbed;
            VideoEmbedContent.Visible = isEmbed;

            if (isEmbed)
            {
                VideoEmbedContent.Text = embedHtml;
            }
            else
            {
                VideoLaunchLink.HRef = languageUrl;
                VideoLaunchLink.Target = target;
            }
        }

        #endregion

        #region Methods (SCORM)

        private void ScormSyncGradebook()
        {
            if (Page.Request["sync"] != "scorm-cloud")
                return;

            var gradebook = Progress.Course.Gradebook;
            if (gradebook == null)
                return;

            try
            {
                var scormCloud = new ScormIntegrator(Organization, User, Guid.Empty);

                var hooks = scormCloud.GetCourseHooks(gradebook.Identifier);
                var activities = scormCloud.GetConditions(gradebook.Identifier);
                var registrations = ScormRegistrationSearch.Select(hooks, activities, User.UserIdentifier);

                bool isNewCompletion = scormCloud.Synchronize(registrations);

                var url = RoutingConfiguration.PortalCourseUrl(Progress.Course.Identifier, Progress.CurrentActivity.Identifier);

                if (isNewCompletion && url != Request.Url.PathAndQuery)
                    Response.Redirect(url);
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                    AppSentry.SentryError(ex);
            }
        }

        private bool ScormContentIsCompleted(string activityHook, Guid userId)
        {
            var registration = ScormRegistrationSearch.Select(activityHook, userId);
            if (registration == null)
                return false;

            if (registration.ScormCompleted == null)
                GetScormCloud().Synchronize(new[] { registration });

            return registration.ScormCompleted != null;
        }

        private ScormIntegrator GetScormCloud()
        {
            var organization = OrganizationSearch.Select(Progress.Course.Organization);

            if (_scormLauncher == null || _scormLauncher.Organization.Identifier != organization.Identifier || _scormLauncher.Activity.ActivityIdentifier != Progress.CurrentActivity.Identifier)
                _scormLauncher = new ScormIntegrator(organization, User, Progress.CurrentActivity.Identifier);

            return _scormLauncher;
        }

        #endregion

        #region Methods (progress commands)

        static readonly ConcurrentDictionary<Guid, object> _mutexes = new ConcurrentDictionary<Guid, object>();

        private void RestartCourse()
        {
            var course = Progress.Course;
            var gradebook = course.Gradebook;

            AreModulesUnlocked = false;
            AreActivitiesUnlocked = false;

            ServiceLocator.ProgressRestarter.Restart(User.Identifier, course.Identifier, DateTimeOffset.UtcNow);

            RedirectToFirstActivity();
        }

        private void RecordProgress(Guid? gradebook, Guid? item, Action<Guid, Guid, Guid> action)
        {
            if (gradebook == null || item == null)
                return;

            var user = User.UserIdentifier;

            try
            {
                object mutex = _mutexes.GetOrAdd(gradebook.Value, key => new object());
                lock (mutex)
                {
                    action(gradebook.Value, user, item.Value);
                }
            }
            finally
            {
                _mutexes.TryRemove(gradebook.Value, out var mutex);
            }
        }

        static private void Start(Guid gradebook, Guid user, Guid item)
        {
            CourseObjectChangeProcessor.EnsureEnrollment(ServiceLocator.SendCommand, ServiceLocator.RecordSearch, gradebook, user, DateTimeOffset.UtcNow);

            var progress = ServiceLocator.ProgressRestarter.GetProgressIdentifier(user, gradebook, item);

            ServiceLocator.SendCommand(new StartProgress(progress, DateTimeOffset.UtcNow));
        }

        static private void Complete(Guid gradebook, Guid user, Guid item)
        {
            CourseObjectChangeProcessor.EnsureEnrollment(ServiceLocator.SendCommand, ServiceLocator.RecordSearch, gradebook, user, DateTimeOffset.UtcNow);

            var progress = ServiceLocator.ProgressRestarter.GetProgressIdentifier(user, gradebook, item);

            ServiceLocator.SendCommand(new CompleteProgress(progress, DateTimeOffset.UtcNow, null, null, null));
        }

        #endregion

        #region Event handlers

        private void DoneButton_Click(object sender, EventArgs e)
        {
            if (Progress.Course.Gradebook != null && Progress.CurrentActivity.GradeItem != null)
                RecordProgress(Progress.Course.Gradebook.Identifier, Progress.CurrentActivity.GradeItem.Identifier, Complete);

            Response.Redirect(Request.RawUrl);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            var model = Progress;

            if (model.CurrentPage > 0
                && model.Course.Gradebook != null
                && model.CurrentActivity.GradeItem != null
                && model.CurrentActivity.Requirement == RequirementType.None
                && (!StringHelper.Equals(model.CurrentActivity.Type, "Assessment") || model.CurrentActivityResult.IsCompleted)
                )
            {
                RecordProgress(model.Course.Gradebook.Identifier, model.CurrentActivity.GradeItem.Identifier, Complete);
            }

            Response.Redirect(model.NextPageUrl);
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            var model = Progress;
            Response.Redirect(Progress.PreviousPageUrl);
        }

        private void RestartCourseButton_Click(object sender, EventArgs e)
        {
            RestartCourse();
        }

        private void UnlockModulesButton_Click(object sender, EventArgs e)
        {
            AreModulesUnlocked = !AreModulesUnlocked;

            RedirectToFirstActivity();
        }

        private void UnlockActivitiesButton_Click(object sender, EventArgs e)
        {
            AreActivitiesUnlocked = !AreActivitiesUnlocked;

            RedirectToFirstActivity();
        }

        #endregion

        #region Methods (helpers)

        private string GetHomeUrl(Guid courseId)
        {
            var url = RelativeUrl.PortalHomeUrl;

            if (ServiceLocator.Partition.IsE03())
            {
                // If this is a CMDS organization, and if the course is NOT referenced by a Web Page,
                // then the home page is the CMDS e-learning catalog.

                if (!ServiceLocator.PageSearch.Exists(x => x.ObjectType == "Course" && x.ObjectIdentifier == courseId))
                {
                    url = "/ui/portal/learning/catalog";

                    if (Page.Master is PortalMaster m)
                        m.OverrideHomeLink(url);
                }
            }

            return url;
        }

        private void RedirectToFirstActivity()
        {
            HttpResponseHelper.Redirect(_state.PortalPage.NavigateUrl ?? _state.PortalPage.Path);
        }

        #endregion
    }
}