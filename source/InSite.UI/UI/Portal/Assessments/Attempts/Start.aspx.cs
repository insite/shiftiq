using System;
using System.Linq;

using Humanizer;

using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Portal.Assessments.Attempts
{
    public partial class Start : PortalBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Properties

        private AttemptUrlBase AttemptUrl
        {
            get => (AttemptUrlBase)ViewState[nameof(AttemptUrl)];
            set => ViewState[nameof(AttemptUrl)] = value;
        }

        private Guid? RegistrationIdentifier
        {
            get => (Guid?)ViewState[nameof(RegistrationIdentifier)];
            set => ViewState[nameof(RegistrationIdentifier)] = value;
        }

        private Guid? LearnerUserIdentifier
        {
            get => (Guid?)ViewState[nameof(LearnerUserIdentifier)];
            set => ViewState[nameof(LearnerUserIdentifier)] = value;
        }

        private int TimeLimit
        {
            get => (int?)ViewState[nameof(TimeLimit)] ?? 0;
            set => ViewState[nameof(TimeLimit)] = value;
        }

        private string Language => CurrentSessionState.Identity.Language ?? Shift.Common.Language.Default;

        #endregion

        #region Fields

        private Form _bankForm;
        private bool _canStart;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            ConfirmLearnerButton.Click += ConfirmLearnerButton_Click;
            LearnerSelector.AutoPostBack = true;
            LearnerSelector.ValueChanged += LearnerSelector_ValueChanged;

            StartButton.Click += StartButton_Click;
        }

        private void LearnerSelector_ValueChanged(object sender, EventArgs e)
        {
            if (!LearnerSelector.HasValue)
                return;

            var user = UserSearch.Select(LearnerSelector.Value.Value);
            PersonFullName.Text = user.FullName;
        }

        private void ConfirmLearnerButton_Click(object sender, EventArgs e)
        {
            LearnerUserIdentifier = LearnerSelector.Value;

            if (LearnerUserIdentifier.HasValue)
                LoadExamView(true);
            else
                AlertMessage.AddMessage(AlertType.Error, "Please select a learner.");

            LearnerSelector.Value = null;

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            LoadExamView(!IsPostBack);

            try
            {
                var attemptId = AttemptHelper.StartAttempt(
                    Organization.OrganizationIdentifier,
                    User.UserIdentifier,
                    LearnerUserIdentifier ?? User.UserIdentifier,
                    _bankForm,
                    RegistrationIdentifier,
                    TimeLimit,
                    Language);

                var redirectUrl = AttemptUrl.GetAnswerUrl(attemptId);

                HttpResponseHelper.Redirect(redirectUrl);
            }
            catch (ApplicationError err)
            {
                MultiView.ActiveViewIndex = -1;

                AlertMessage.AddMessage(AlertType.Error, $"<b>Can't start exam.</b> {err.Message}");
            }
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var action = LoadExamView(!IsPostBack);

            if (!_canStart)
                DisableStartButton();

            else if (action == null && IsAutoStartEnabled())
                StartButton_Click(this, new EventArgs());

            ConfigureLayout();
        }

        private bool IsAutoStartEnabled()
        {
            return !_bankForm.ThirdPartyAssessmentIsEnabled && Request.QueryString["auto-start"] == "enabled";
        }

        private void DisableStartButton()
        {
            StartButton.Enabled = false;
        }

        private void ConfigureLayout()
        {
            PortalMaster.SidebarVisible(false);
            PortalMaster.HideBreadcrumbsAndTitle();
        }

        private AttemptHelper.IAction LoadExamView(bool isInit)
        {
            AttemptHelper.IAction action;

            var attemptId = ValueConverter.ToGuidNullable(Request.QueryString["attempt"]);

            if (Identity.IsImpersonating)
            {
                action = AttemptHelper.GetErrorResult(
                    "Permission Denied",
                    "You cannot start an assessment while impersonating another user.");
            }
            else if (Guid.TryParse(Request.QueryString["resource"], out var resourceId))
            {
                action = LoadResource(resourceId, attemptId, isInit, true);
            }
            else if (Guid.TryParse(Request.QueryString["registration"], out var registrationId))
            {
                action = LoadRegistration(registrationId, attemptId, isInit);
            }
            else if (Guid.TryParse(Request.QueryString["form"], out var formId))
            {
                action = LoadForm(formId, attemptId, isInit);
            }
            else
                action = AttemptHelper.GetErrorResult(
                    "Invalid URL",
                    "A valid resource identifier is required.");

            if (action == null && LockedGradebookHelper.HasLockedGradebook(_bankForm.Identifier, _bankForm.Hook))
                action = AttemptHelper.GetErrorResult("Gradebook Locked", "The gradebook is locked, please contact the administrator for details.");

            if (action != null)
            {
                MultiView.ActiveViewIndex = -1;
                action.Execute(AlertMessage, Notification);
            }

            return action;
        }

        private AttemptHelper.IAction LoadResource(Guid resourceId, Guid? attemptId, bool isInit, bool allowAssessor)
        {
            var actionResult = AttemptHelper.LoadResource(resourceId, out _bankForm, out var contentStyle);
            if (actionResult != null)
                return actionResult;

            var url = AttemptUrlResource.Create(resourceId, _bankForm.Identifier, attemptId);

            return OnFormLoaded(url, contentStyle, isInit, allowAssessor);
        }

        private AttemptHelper.IAction LoadRegistration(Guid registrationId, Guid? attemptId, bool isInit)
        {
            var resourceId = Guid.Empty;

            var registration = ServiceLocator.RegistrationSearch.GetRegistration(registrationId);
            if (registration != null
                && registration.OrganizationIdentifier == Organization.Identifier
                && registration.CandidateIdentifier == User.UserIdentifier
                && registration.ExamFormIdentifier.HasValue)
            {
                var resources = ServiceLocator.PageSearch.Bind(x => x.PageIdentifier,
                    x => x.ObjectType == "Assessment" && x.ObjectIdentifier == registration.ExamFormIdentifier.Value);

                if (resources.Length == 1)
                {
                    resourceId = resources[0];

                    RegistrationIdentifier = registration.RegistrationIdentifier;

                    if (registration.ExamTimeLimit.HasValue)
                        TimeLimit = registration.ExamTimeLimit.Value;
                }
            }

            return LoadResource(resourceId, attemptId, isInit, false);
        }

        private AttemptHelper.IAction LoadForm(Guid formId, Guid? attemptId, bool isInit)
        {
            var url = AttemptUrlForm.Create(formId, attemptId);

            var actionResult = AttemptHelper.LoadForm(url, out _bankForm);
            if (actionResult != null)
                return actionResult;

            return OnFormLoaded(url, null, isInit, true);
        }

        private AttemptHelper.IAction OnFormLoaded(AttemptUrlBase url, string contentStyle, bool isInit, bool allowAssessor)
        {
            var assessorEnabled = allowAssessor && _bankForm.ThirdPartyAssessmentIsEnabled;
            var learnerId = assessorEnabled ? LearnerUserIdentifier : User.Identifier;

            if (learnerId.HasValue)
            {
                MultiView.SetActiveView(StartExamView);

                var actionResult = AttemptHelper.LoadAttemptStart(_bankForm, learnerId.Value, url);
                if (actionResult != null)
                    return actionResult;
            }
            else
            {
                MultiView.SetActiveView(SelectLearnerView);
                LearnerSelector.Value = null;
            }

            if (isInit)
            {
                SetInputValues(_bankForm, learnerId);
                SetStyle(contentStyle);

                AttemptUrl = url;
            }

            return null;
        }

        private void SetStyle(string style)
        {
            var hasStyle = style.IsNotEmpty();
            ContentStyle.Visible = hasStyle;
            ContentStyle.Text = hasStyle ? $"<style type=\"text/css\">{style}</style>" : null;
        }

        private void SetInputValues(Form bankForm, Guid? learnerId)
        {
            var spec = bankForm.Specification;

            _canStart = true;

            if (spec.IsTabTimeLimitAllowed && spec.TabTimeLimit == SpecificationTabTimeLimit.AllTabs)
            {
                var timeSum = 0;

                foreach (var section in bankForm.Sections)
                {
                    if (section.TimeLimit <= 0)
                    {
                        timeSum = 0;
                        _canStart = false;

                        AlertMessage.AddMessage(
                            AlertType.Error,
                            "Assessment cannot be started due to incorrect configuration: " +
                            "one or more sections in the form have no defined Time Limit.");

                        break;
                    }

                    timeSum += section.TimeLimit;
                }

                TimeLimit = timeSum;
            }
            else if (TimeLimit <= 0)
            {
                TimeLimit = bankForm.Invigilation.TimeLimit;
            }

            SetFormInputValues(bankForm);

            if (!SetLearnerInputValues(learnerId))
                _canStart = false;

            SetRegistrationInputValues(RegistrationIdentifier);

            var composedQuestions = bankForm.GetQuestions()
                .Where(x => x.Type.IsComposed())
                .Select(x => x.Identifier)
                .ToArray();

            if (composedQuestions.Length > 0)
            {
                var existing = ServiceLocator.BankSearch.GetQuestionsNotConnectedToRubrics(composedQuestions);
                if (existing.Count > 0)
                {
                    AlertMessage.AddMessage(AlertType.Warning, $"Your assessment is pending review by an administrator.");

                    _canStart = false;
                }
            }
        }

        private void SetFormInputValues(Form bankForm)
        {
            var content = bankForm.Content;
            var invigilation = bankForm.Invigilation;

            KioskMode.Visible = invigilation.IsKioskModeRequired;

            AssessmentTitle.InnerText = content.Title?[Language] ?? content.Title?.Default;

            var instructions = content.InstructionsForOnline?[Language] ?? content.InstructionsForOnline?.Default;

            InstructionsLiteral.Text = !string.IsNullOrEmpty(instructions)
                ? $"<div class=\"m-t-0 m-b-md\">{Markdown.ToHtml(instructions)}</div><div style='height:40px;'>&nbsp;</div>"
                : null;

            TimeLimitField.Visible = invigilation.TimeLimitPerSession > 0 && invigilation.AttemptLimitPerSession > 0;
            if (invigilation.TimeLimitPerSession > 0 && invigilation.AttemptLimitPerSession > 0)
            {
                TimeLimitPerSession.Text = invigilation.TimeLimitPerSession.Minutes().Humanize();
                AttemptLimitPerSession.Text = "failed attempt".ToQuantity(invigilation.AttemptLimitPerSession);
                TimeLimitPerLockout.Text = (invigilation.TimeLimitPerLockout <= 0 ? 24 * 60 : invigilation.TimeLimitPerLockout).Minutes().Humanize();
            }

            var hasTimeLimit = TimeLimit > 0;

            TimerField.Visible = hasTimeLimit;

            if (hasTimeLimit)
            {
                var timeLimit = new TimeSpan(0, TimeLimit, 0);
                TimerLiteral.Text = Translate("You have a time limit of {0} to complete the exam.").Format(timeLimit.ToString("hh':'mm':'ss"));
            }
        }

        private bool SetLearnerInputValues(Guid? learnerId)
        {
            var isValid = true;
            if (!learnerId.HasValue)
                return isValid;

            var assessorId = User.UserIdentifier;
            var hasAssessor = assessorId != learnerId.Value;

            AssessorContainer.Visible = hasAssessor;

            if (hasAssessor)
            {
                var assessor = ServiceLocator.ContactSearch.GetPerson(assessorId, Organization.Identifier);
                if (assessor == null)
                {
                    AlertMessage.AddMessage(AlertType.Error, "Assessor not found.");
                    AssessorFirstName.Text = "N/A";
                    AssessorLastName.Text = "N/A";
                    AssessorEmail.Text = "N/A";

                    isValid = false;
                }
                else
                {
                    AssessorFirstName.Text = assessor.UserFirstName;
                    AssessorLastName.Text = assessor.UserLastName;
                    AssessorEmail.Text = assessor.UserEmail;
                }
            }

            var learner = ServiceLocator.ContactSearch.GetPerson(learnerId.Value, Organization.Identifier);
            if (learner == null)
            {
                AlertMessage.AddMessage(AlertType.Error, "Learner not found.");
                LearnerFirstName.Text = "N/A";
                LearnerLastName.Text = "N/A";
                LearnerEmail.Text = "N/A";
                LearnerCodeField.Visible = false;

                isValid = false;
            }
            else
            {
                LearnerFirstName.Text = learner.UserFirstName;
                LearnerLastName.Text = learner.UserLastName;
                LearnerEmail.Text = learner.UserEmail;

                LearnerCodeField.Visible = learner.PersonCode.IsNotEmpty();
                LearnerCode.Text = learner.PersonCode;
            }

            return isValid;
        }

        private void SetRegistrationInputValues(Guid? registrationId)
        {
            var @event = registrationId.HasValue
                ? ServiceLocator.RegistrationSearch.GetRegistration(registrationId.Value, x => x.Event)?.Event
                : null;

            var eventScheduled = @event?.EventScheduledStart;
            var eventTimeLimit = @event?.ExamDurationText;

            EventScheduledField.Visible = eventScheduled.HasValue;

            if (eventScheduled.HasValue)
            {
                var timeZone = CurrentSessionState.Identity.User.TimeZone;
                var scheduled = TimeZoneInfo.ConvertTime(eventScheduled.Value, timeZone);
                var timeZoneAbbrv = TimeZones.GetAbbreviation(timeZone).GetAbbreviation(eventScheduled.Value);

                EventScheduled.Text = $"{scheduled:MMM d, yyyy h:mm tt} {timeZoneAbbrv}";
            }

            EventTimeLimitField.Visible = eventTimeLimit.IsNotEmpty();
            EventTimeLimit.Text = eventTimeLimit;
        }

        #endregion

        #region Route Hierarchy

        private BreadcrumbDataItem BreadcrumbParent
        {
            get => (BreadcrumbDataItem)ViewState[nameof(BreadcrumbParent)];
            set => ViewState[nameof(BreadcrumbParent)] = value;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            BreadcrumbParent;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            parent is BreadcrumbDataItem bRoute ? bRoute.NavigationParameters : null;

        #endregion
    }
}