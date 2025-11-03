using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

using Humanizer;

using InSite.Admin.Assessments.Sections.Models;
using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Write;
using InSite.Application.Records.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Constant;

using BankQuestion = InSite.Domain.Banks.Question;
using BankSection = InSite.Domain.Banks.Section;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public static class AttemptHelper
    {
        #region Classes

        public interface IAction
        {
            void Execute(Alert errorAlert, ITextControl notification);
        }

        public sealed class ActionError : IAction
        {
            public string Error { get; }

            public ActionError(string error)
            {
                Error = error.NullIfEmpty()
                    ?? throw new ArgumentNullException(nameof(error));
            }

            public void Execute(Alert errorAlert, ITextControl notification)
            {
                errorAlert.AddMessage(AlertType.Error, Error);
            }
        }

        public sealed class ActionRedirect : IAction
        {
            public string Url { get; }

            public ActionRedirect(string url)
            {
                Url = url.NullIfEmpty()
                    ?? throw new ArgumentNullException(nameof(url));
            }

            public void Execute(Alert errorAlert, ITextControl notification)
            {
                HttpResponseHelper.Redirect(Url);
            }
        }

        public sealed class ActionNotification : IAction
        {
            private string _html;
            private string _closeUrl;

            public ActionNotification(AlertType type, string message, string closeUrl)
            {
                if (message.IsEmpty())
                    throw new ArgumentNullException(nameof(message));

                string alertName;
                if (type == AlertType.Information)
                    alertName = "info";
                else if (type == AlertType.Error)
                    alertName = "error";
                else if (type == AlertType.Success)
                    alertName = "success";
                else if (type == AlertType.Warning)
                    alertName = "warning";
                else
                    throw new ArgumentException($"Unexpected argument value: {nameof(type)} = {type.GetName()}");

                _html = $"<div class='alert alert-{alertName}'>{message}</div>";

                _closeUrl = closeUrl.NullIfEmpty()
                    ?? throw new ArgumentNullException(nameof(closeUrl));
            }

            public void Execute(Alert errorAlert, ITextControl notification)
            {
                notification.Text = $"{_html}<div><a class='btn btn-default' href='{_closeUrl}'><i class='fas fa-ban'></i> Close</a></div>";
            }
        }

        private class AssessmentResult
        {
            public Guid? ActivityIdentifier { get; set; }
            public string ActivityType { get; set; }

            public Guid? ResourceIdentifier { get; set; }

            public Guid? AssessmentFormIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }
        }

        #endregion

        #region Fields

        private static readonly Regex _sebUaPatternRegex = new Regex("(?:SEB)/(?<Version>[0-9\\.]+)", RegexOptions.Compiled);

        #endregion

        #region Methods (Commands)

        public static Guid StartAttempt(Guid organizationId, Guid assessorId, Guid learnerId, Form bankForm, Guid? registrationId, int timeLimit, string language)
        {
            var questions = CreateAttemptQuestions(bankForm, true, language);
            if (questions.Length == 0)
                throw new ApplicationError(
                    $"The exam form assigned to this assessment " +
                    $"(Form Asset {bankForm.Asset}: \"{(bankForm.Content.Title?.Default).IfNullOrEmpty(bankForm.Name)}\") " +
                    $"does not contain any questions that match the criteria specified for it.");

            var attemptId = UniqueIdentifier.Create();
            var spec = bankForm.Specification;
            var bankId = spec.Bank.Identifier;
            var config = new AttemptConfiguration
            {
                TimeLimit = timeLimit,
                Language = language,
                SectionsAsTabs = spec.SectionsAsTabsEnabled,
                TabNavigation = spec.TabNavigationEnabled,
                SingleQuestionPerTab = spec.SingleQuestionPerTabEnabled,
                TabTimeLimit = spec.TabTimeLimit
            };
            var sections = AttemptStarter.CreateSections(bankForm);

            var start = new StartAttempt(
                attemptId, organizationId, bankId, bankForm.Identifier, assessorId, learnerId, registrationId, HttpContext.Current?.Request.UserAgent,
                config, sections, questions);
            ServiceLocator.SendCommand(start);

            var analyze = new AnalyzeForm(bankId, bankForm.Identifier);
            ServiceLocator.SendCommand(analyze);

            ProgramHelper.EnrollLearnerByObjectId(organizationId, learnerId, bankForm.Identifier);

            ServiceLocator.ProgramStore.TaskViewed(learnerId, organizationId, bankForm.Identifier);

            if(bankForm.WhenAttemptStartedNotifyAdminMessageIdentifier.HasValue)
                SendBankNotification(organizationId, bankForm, bankForm.WhenAttemptStartedNotifyAdminMessageIdentifier.Value, learnerId);

            return attemptId;
        }

        public static bool CompleteAttempt(QAttempt attempt)
        {
            if (attempt.AttemptSubmitted.HasValue)
                return false;

            var grade = ServiceLocator.AttemptSearch.GetAttemptQuestionTypes(attempt.AttemptIdentifier)
                .Select(x => x.ToEnum<QuestionItemType>()).All(x => !x.IsComposed());

            ProgramEnrollmentCompletion(attempt);

            var enrollmentsToCheck = ServiceLocator.ProgramStore.TaskCompleted(attempt.LearnerUserIdentifier, CurrentSessionState.Identity.Organization.OrganizationIdentifier, attempt.FormIdentifier);
            foreach (var enrollment in enrollmentsToCheck)
                ServiceLocator.ProgramService.CompletionOfProgramAchievement(enrollment.ProgramIdentifier, enrollment.LearnerIdentifier, enrollment.OrganizationIdentifier);

            ServiceLocator.SendCommand(new SubmitAttempt(attempt.AttemptIdentifier, HttpContext.Current?.Request.UserAgent, grade));

            return true;
        }

        #endregion

        #region Methods (Loading)

        public static IAction LoadResource(Guid pageId, out Form bankForm, out string contentStyle)
        {
            bankForm = null;
            contentStyle = null;

            var lesson = ServiceLocator.PageSearch.BindFirst(
                x => new
                {
                    x.OrganizationIdentifier,
                    x.PageIdentifier,
                    x.IsHidden,
                    AssessmentIdentifier = x.ObjectType == "Assessment" ? x.ObjectIdentifier : null
                },
                x => x.PageIdentifier == pageId);

            if (lesson == null)
                return GetErrorResult();

            if (lesson.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                if (!ServiceLocator.Partition.IsE03())
                    return GetErrorResult();

                var organization = OrganizationSearch.Select(lesson.OrganizationIdentifier);
                if (!ServiceLocator.Partition.IsE03())
                    return GetErrorResult();
            }

            if (!lesson.AssessmentIdentifier.HasValue)
                return GetErrorResult();

            bankForm = ServiceLocator.BankSearch.GetFormData(lesson.AssessmentIdentifier.Value);

            if (!ValidateForm(bankForm, out var examError))
                return examError;

            return null;

            IAction GetErrorResult()
            {
                return new ActionError(
                    "<strong>Assessment Not Found</strong>. " +
                    "Please contact your administrator with the steps you followed to arrive at this error message.");
            }
        }

        public static IAction LoadForm(AttemptUrlBase url, out Form bankForm)
        {
            bankForm = null;

            var assessment = ServiceLocator.PageSearch.BindFirst(
                x => new AssessmentResult
                {
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    ResourceIdentifier = x.PageIdentifier,
                    AssessmentFormIdentifier = x.ObjectIdentifier
                },
                x => x.ObjectType == "Assessment" && x.ObjectIdentifier == url.FormIdentifier);

            if (assessment == null)
            {
                assessment = CourseSearch.BindActivityFirst(x => new AssessmentResult
                {
                    OrganizationIdentifier = x.Module.Unit.Course.OrganizationIdentifier,
                    ActivityIdentifier = x.ActivityIdentifier,
                    ActivityType = x.ActivityType,
                    AssessmentFormIdentifier = x.AssessmentFormIdentifier
                },
                x => x.AssessmentFormIdentifier == url.FormIdentifier);

                if (assessment != null && TGroupPermissionSearch.IsAccessDenied(assessment.ActivityIdentifier.Value, CurrentSessionState.Identity))
                    assessment = null;
            }

            if (assessment == null)
                return GetErrorResult();

            if (assessment.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                if (!ServiceLocator.Partition.IsE03())
                    return GetErrorResult();

                var assessmentOrganization = OrganizationSearch.Select(assessment.OrganizationIdentifier);
                if (!ServiceLocator.Partition.IsE03())
                    return GetErrorResult();
            }

            if (!assessment.AssessmentFormIdentifier.HasValue)
                return GetErrorResult();

            bankForm = ServiceLocator.BankSearch.GetFormData(assessment.AssessmentFormIdentifier.Value);

            if (!ValidateForm(bankForm, out var examError))
                return examError;

            return null;

            IAction GetErrorResult()
            {
                return new ActionError(
                    "<strong>Assessment Not Found</strong>. " +
                    "Please contact your administrator with the steps you followed to arrive at this error message.");
            }
        }

        public static IAction LoadAttemptStart(Form form, Guid learnerId, AttemptUrlBase url)
        {
            var isFormOpened = !form.Invigilation.Opened.HasValue || form.Invigilation.Opened.Value <= DateTimeOffset.Now;
            if (!isFormOpened)
                return ActionFormNotOpened(form, url);

            var summary = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(form.Identifier, learnerId)
                ?? new TLearnerAttemptSummary();

            if (!ServiceLocator.Partition.IsE03() && summary.AttemptLastPassedIdentifier.HasValue)
            {
                if (url is AttemptUrlResource || url is AttemptUrlForm)
                {
                    return new ActionRedirect(url.GetResultUrl(summary.AttemptLastPassedIdentifier.Value));
                }
                else
                    throw new ApplicationError($"Unexpected URL type: {url.GetType()}");
            }

            var isLastAttempt = !url.AttemptID.HasValue || summary.AttemptLastStartedIdentifier == url.AttemptID;
            if (!isLastAttempt)
                return new ActionRedirect(url.GetResultUrl());

            var isAllAttemptsGraded = summary.AttemptSubmittedCount > summary.AttemptGradedCount;
            if (isAllAttemptsGraded)
                return new ActionRedirect(url.GetResultUrl(summary.AttemptLastSubmittedIdentifier.Value));

            var isLimitExceeded = form.Invigilation.AttemptLimit > 0 && summary.AttemptSubmittedCount >= form.Invigilation.AttemptLimit;
            if (isLimitExceeded)
                return new ActionRedirect(url.GetResultUrl(summary.AttemptLastSubmittedIdentifier.Value));

            var hasActiveAttempt = summary.AttemptLastStartedIdentifier.HasValue
                && summary.AttemptLastStartedIdentifier != summary.AttemptLastSubmittedIdentifier;
            if (hasActiveAttempt)
                return new ActionRedirect(url.GetAnswerUrl(summary.AttemptLastStartedIdentifier.Value));

            var isFormClosed = form.Invigilation.Closed.HasValue && form.Invigilation.Closed.Value < DateTimeOffset.Now;
            if (isFormClosed)
            {
                if (summary.AttemptLastStartedIdentifier.HasValue)
                    return new ActionRedirect(url.GetResultUrl(summary.AttemptLastStartedIdentifier.Value));

                return ActionFormClosed(form, url);
            }

            if (form.Invigilation.AttemptLimitPerSession > 0 && summary.AttemptTotalCount > 0)
            {
                DateTimeOffset? dateLocked = null;
                double lockMinutes = form.Invigilation.TimeLimitPerLockout > 0 ? form.Invigilation.TimeLimitPerLockout : 24 * 60;

                if (form.Invigilation.TimeLimitPerSession > 0)
                {
                    var attempts = ServiceLocator.AttemptSearch.GetAttempts(form.Identifier, learnerId)
                        .OrderByDescending(x => x.AttemptNumber).Take(form.Invigilation.AttemptLimitPerSession)
                        .ToArray();

                    if (attempts.Length == form.Invigilation.AttemptLimitPerSession)
                    {
                        if (attempts.Length == 1)
                        {
                            var last = attempts[0];
                            dateLocked = last.AttemptGraded ?? last.AttemptSubmitted ?? last.AttemptPinged ?? last.AttemptStarted;
                        }
                        else
                        {
                            var first = attempts[attempts.Length - 1];
                            var last = attempts[0];
                            var minutes = (last.AttemptStarted.Value - first.AttemptStarted.Value).TotalMinutes;

                            if (minutes <= form.Invigilation.TimeLimitPerSession)
                                dateLocked = last.AttemptGraded ?? last.AttemptSubmitted ?? last.AttemptPinged ?? last.AttemptStarted;
                        }
                    }
                }
                else
                {
                    var count = summary.AttemptTotalCount - summary.AttemptVoidedCount;

                    var isLocked = count > 0 && count % form.Invigilation.AttemptLimitPerSession == 0;
                    if (isLocked)
                        dateLocked = summary.AttemptLastSubmitted;
                }

                if (dateLocked.HasValue)
                {
                    var passedMinutes = (DateTimeOffset.Now - dateLocked.Value).TotalMinutes;
                    if (passedMinutes <= lockMinutes)
                    {
                        var unlockTime = dateLocked.Value.AddMinutes(lockMinutes).Humanize();

                        return GetErrorResult("This exam is locked", $"Exam will be unlocked in {unlockTime}");
                    }
                }
            }

            return null;
        }

        public static IAction LoadAttemptAnswer(Form form, AttemptUrlBase url, out QAttempt attempt)
        {
            attempt = null;

            if (!url.AttemptID.HasValue)
                return ActionInvalidAttemptKey();

            var isFormOpened = !form.Invigilation.Opened.HasValue || form.Invigilation.Opened.Value <= DateTimeOffset.Now;
            var isFormClosed = form.Invigilation.Closed.HasValue && form.Invigilation.Closed.Value < DateTimeOffset.Now;
            if (!isFormOpened || isFormClosed)
                return new ActionRedirect(url.GetResultUrl());

            var entity = ServiceLocator.AttemptSearch.GetAttempt(url.AttemptID.Value, x=>x.Form, x=>x.Form.Bank);
            if (entity == null)
                return ActionInvalidAttemptKey();

            var user = CurrentSessionState.Identity.User;
            if (entity.AssessorUserIdentifier != user.UserIdentifier && entity.LearnerUserIdentifier != user.UserIdentifier)
                return ActionInvalidAttemptKey();

            var summary = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(form.Identifier, entity.LearnerUserIdentifier)
                ?? new TLearnerAttemptSummary();

            var isAttemptStarted = summary.AttemptLastStartedIdentifier.HasValue
                && url.AttemptID.Value == summary.AttemptLastStartedIdentifier.Value;
            if (!isAttemptStarted)
                return new ActionRedirect(url.GetResultUrl());

            if (entity.AttemptSubmitted.HasValue)
                return new ActionRedirect(url.GetResultUrl());

            if (!entity.AttemptStarted.HasValue)
                return new ActionRedirect(url.GetStartUrl(entity.AttemptIdentifier));

            attempt = entity;

            if (attempt.AttemptTimeLimit.HasValue && attempt.AttemptTimeLimit.Value > 0)
            {
                // Calculate the total number of minutes elapsed between the start of the attempt and the last ping. If this exceeds the time limit
                // assigned to the attempt then consider it complete. Otherwise, allow the learer to resume.

                var timeElapsed = ((double?)entity.AttemptDuration ?? 0).Seconds();
                if (timeElapsed.TotalMinutes >= attempt.AttemptTimeLimit.Value)
                    return new ActionRedirect(url.GetResultUrl());
            }

            var pingDate = entity.AttemptPinged ?? entity.AttemptStarted ?? DateTimeOffset.UtcNow;
            var pingInterval = attempt.AttemptPingInterval ?? AttemptConfiguration.DefaultPingInterval;
            if ((DateTimeOffset.UtcNow - pingDate).TotalSeconds > pingInterval * 2)
            {
                ServiceLocator.SendCommand(new ResumeAttempt(entity.AttemptIdentifier, null));
                ServiceLocator.SendCommand(new AnalyzeForm(form.Specification.Bank.Identifier, form.Identifier));
            }

            return null;
        }

        public static IAction LoadAttemptResult(Form form, AttemptUrlBase url, out QAttempt attempt)
        {
            attempt = null;

            var currentAttempt = !url.AttemptID.HasValue
                ? null
                : ServiceLocator.AttemptSearch.GetAttempt(url.AttemptID.Value);

            if (currentAttempt == null)
                return ActionInvalidAttemptKey();

            if (!currentAttempt.AttemptStarted.HasValue)
                return new ActionRedirect(url.GetStartUrl(currentAttempt.AttemptIdentifier));

            attempt = currentAttempt;

            return null;
        }

        #endregion

        #region Methods (Loading Helpers)

        private static bool ValidateForm(Form form, out IAction errorResult)
        {
            errorResult = null;

            if (form == null)
            {
                errorResult = GetErrorResult(
                    "Exam Form Not Found",
                    "<p>This exam form is not yet published. Please contact your administrator.</p>");

                return false;
            }

            if (form.Specification.Type == SpecificationType.Static)
            {
                var questions = form.Sections.SelectMany(x => x.Fields.Select(y => y.Question)).ToArray();

                if (questions.Length == 0)
                {
                    errorResult = GetErrorResult(
                        "Exam Form Missing Questions",
                        "<p>This exam form contains no questions.</p>");

                    return false;
                }

                var hasEmptyQuestion = questions.Any(
                    x => (x.Type.IsRadioList() || x.Type.IsCheckList()) && x.Options.Count == 0
                         || x.Type == QuestionItemType.Matching && x.Matches.IsEmpty
                         || x.Type == QuestionItemType.Likert && !x.Likert.HasOptions
                         || x.Type.IsHotspot() && x.Hotspot.Options.Count == 0
                         || x.Type == QuestionItemType.Ordering && x.Ordering.Options.Count == 0);

                if (hasEmptyQuestion)
                {
                    errorResult = GetErrorResult(
                        "Exam Form Missing Options",
                        "<p>This exam form contains questions with no answer options. Please contact your administrator.</p>");

                    return false;
                }
            }

            if (form.Publication.IsPublished && form.Invigilation.IsSafeExamBrowserRequired)
            {
                var userAgent = HttpContext.Current.Request.UserAgent;

                if (GetSebVersion(userAgent) == null)
                {
                    errorResult = GetErrorResult("Invalid Browser",
                        $"You must use the <a href='https://www.safeexambrowser.org/download_en.html' target='_blank'>Safe Exam Browser</a> to access this exam.");

                    return false;
                }

                var suffix = CurrentSessionState.Identity.Organization.PlatformCustomization.SafeExamBrowserUserAgentSuffix;
                if (!string.IsNullOrEmpty(suffix) && !userAgent.EndsWith(" " + suffix))
                {
                    errorResult = GetErrorResult("Invalid Browser Configuration",
                        $"Your Safe Exam Browser is not configured for access to this exam.");

                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Methods (Commands Helpers)

        public static void SendBankNotification(Guid organizationId, Form bankForm, Guid messageIdentifier, Guid learnerUserIdentifier, decimal? attemptScore = null)
        {
            var learner = ServiceLocator.PersonSearch.GetPerson(learnerUserIdentifier, organizationId, x => x.User);
            var score = attemptScore ?? 0;
            var email = learner?.User?.Email;

            var notification = new BankNotification
            {
                OriginOrganization = organizationId,
                MessageIdentifier = messageIdentifier,

                LearnerPersonCode = learner?.PersonCode,
                LearnerName = learner?.FullName,
                LearnerEmail = email,
                AssessmentFormName = bankForm.Name,
                AssessmentAttemptScore = $"{score:p0}"
            };

            ServiceLocator.AlertMailer.Send(notification, null);
        }

        public static AttemptQuestion[] CreateAttemptQuestions(Form bankForm, bool allowRandomization, string language)
        {
            var questions = new List<BankQuestion>();
            var sectionMapping = new Dictionary<Guid, Tuple<BankSection, List<BankQuestion>>>();
            var sectionIndexMapping = new Dictionary<Guid, int>();
            var setMapping = new Dictionary<Guid, Tuple<Set, List<int>>>();

            if (bankForm.Specification.Type == SpecificationType.Static)
            {
                for (var sectionIndex = 0; sectionIndex < bankForm.Sections.Count; sectionIndex++)
                {
                    var section = bankForm.Sections[sectionIndex];

                    foreach (var field in section.Fields)
                    {
                        var index = questions.Count;
                        var question = field.Question;
                        var set = question.Set;

                        if (!sectionMapping.ContainsKey(section.Identifier))
                            sectionMapping.Add(section.Identifier, new Tuple<BankSection, List<BankQuestion>>(section, new List<BankQuestion>()));

                        if (!setMapping.ContainsKey(set.Identifier))
                            setMapping.Add(set.Identifier, new Tuple<Set, List<int>>(set, new List<int>()));

                        questions.Add(question);
                        sectionMapping[section.Identifier].Item2.Add(question);
                        sectionIndexMapping.Add(question.Identifier, sectionIndex);
                        setMapping[set.Identifier].Item2.Add(index);
                    }
                }
            }
            else
            {
                var criteria = bankForm.Specification.Criteria;

                if (criteria.Count > 0)
                {
                    var questionFilter = new QuestionFilterHelper(criteria, null, false);
                    var questionGroups = questionFilter.GetResult();

                    questions.AddRange(questionGroups.SelectMany(x => x.Item2));
                }
                else
                {
                    foreach (var set in bankForm.Specification.Bank.Sets)
                    {
                        if (!setMapping.ContainsKey(set.Identifier))
                            setMapping.Add(set.Identifier, new Tuple<Set, List<int>>(set, new List<int>()));

                        foreach (var question in set.Questions)
                        {
                            var index = questions.Count;
                            questions.Add(question);
                            setMapping[set.Identifier].Item2.Add(index);
                        }
                    }
                }
            }

            var questionIndexes = new int?[questions.Count];

            if (allowRandomization)
            {
                foreach (var sm in setMapping.Values)
                {
                    var set = sm.Item1;
                    var indexes = sm.Item2;

                    if (!set.Randomization.Enabled)
                        continue;

                    var count = set.Randomization.Count <= 0 || set.Randomization.Count > indexes.Count
                        ? indexes.Count
                        : set.Randomization.Count;

                    var random = new Random();

                    while (count > 0)
                    {
                        var randomIndex = random.Next(count--);
                        var index1 = indexes[randomIndex];
                        var index2 = indexes[count];

                        var buffer = questionIndexes[index1] ?? index1;
                        questionIndexes[index1] = questionIndexes[index2] ?? index2;
                        questionIndexes[index2] = buffer;
                    }
                }
            }

            var hiddenQuestions = new HashSet<Guid>();

            foreach (var sm in sectionMapping.Values)
            {
                var sieve = sm.Item1.Criterion;
                if (string.IsNullOrEmpty(sieve.TagFilter))
                    continue;

                var filter = QuestionDisplayFilter.Parse(sieve.TagFilter);

                var sectionQuestions = sm.Item2.ToArray();
                sectionQuestions.Shuffle();

                for (var i = 0; i < sectionQuestions.Length; i++)
                {
                    var question = sectionQuestions[i];
                    var tag = question.Classification.Tag != null ? filter[question.Classification.Tag] : null;

                    if (tag != null && tag.Allows)
                        tag.Increment();
                    else
                        hiddenQuestions.Add(question.Identifier);
                }
            }

            var displayCount = bankForm.Specification.QuestionLimit > 0
                            && bankForm.Specification.QuestionLimit < questions.Count
                ? bankForm.Specification.QuestionLimit
                : questions.Count;
            var result = new List<AttemptQuestion>();

            for (var i = 0; i < questions.Count && result.Count < displayCount; i++)
            {
                var index = questionIndexes[i] ?? i;
                var question = questions[index];

                if (hiddenQuestions.Contains(question.Identifier))
                    continue;

                var attemptQuestion = AttemptStarter.CreateQuestion(question, allowRandomization, language);

                if (sectionIndexMapping.TryGetValue(attemptQuestion.Identifier, out var sectionIndex))
                    attemptQuestion.Section = sectionIndex;

                result.Add(attemptQuestion);
            }

            return result.ToArray();
        }

        #endregion

        #region Methods (Helpers)

        private static IAction ActionInvalidAttemptKey()
        {
            return GetErrorResult(
                "Invalid URL",
                $"<p>The attempt key is invalid.</p>");
        }

        private static IAction ActionFormNotOpened(Form form, AttemptUrlBase url)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var dateOpened = form.Invigilation.Opened.Value.Format(timeZone);

            return new ActionNotification(
                AlertType.Information,
                $"This exam is scheduled to open {dateOpened}.",
                url.GetReturnUrl());
        }

        private static IAction ActionFormClosed(Form form, AttemptUrlBase url)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var dateClosed = form.Invigilation.Closed.Value.Format(timeZone);

            return new ActionNotification(
                AlertType.Information,
                $"This exam closed {dateClosed}.",
                url.GetReturnUrl());
        }

        public static string GetRegistrationStartUrl(Guid registrationId) =>
            $"/ui/portal/assessments/attempts/start?registration={registrationId}";

        public static IAction GetErrorResult(string title, string message) =>
            new ActionError($"<b>{title}.</b> {message}");

        public static string GetSebVersion(string userAgent)
        {
            if (!userAgent.IsEmpty())
            {
                var match = _sebUaPatternRegex.Match(userAgent);
                if (match.Success)
                    return match.Groups["Version"].Value;
            }

            return null;
        }

        private static void ProgramEnrollmentCompletion(QAttempt attempt)
        {
            var programs = ServiceLocator.ProgramSearch.GetProgramIds(attempt.FormIdentifier);
            if (programs != null && programs.Count > 0)
            {
                foreach (var program in programs)
                {
                    var values = ServiceLocator.ProgramSearch.GetProgramValues(program, attempt.FormIdentifier);
                    if (values == null)
                        return;

                    var organizationId = CurrentSessionState.Identity.Organization.OrganizationIdentifier;

                    if ((values.CompletionTaskIdentifier.HasValue && values.TaskIdentifier.HasValue && values.TaskIdentifier.Value.Equals(values.CompletionTaskIdentifier.Value)) ||
                        ServiceLocator.ProgramSearch.IsProgramFullyCompletedByLearner(values.ProgramIdentifier, attempt.LearnerUserIdentifier))
                    {
                        var user = ServiceLocator.ContactSearch.GetUser(attempt.LearnerUserIdentifier);
                        var person = ServiceLocator.ContactSearch.GetPerson(attempt.LearnerUserIdentifier, organizationId);
                        if (person != null)
                        {
                            var email = EmailAddress.GetEnabledEmail(user.UserEmail, person.UserEmailEnabled, user.UserEmailAlternate, person.UserEmailAlternateEnabled);
                            if (email.IsEmpty())
                                return;

                            if (values.AchievementIdentifier.HasValue)
                                ProgramHelper.SendGrantCommands(TriggerEffectCommand.Grant, organizationId, values.AchievementIdentifier.Value, user.UserIdentifier);

                            var enrollment = new ProgramEnrollment(ServiceLocator.AlertMailer,
                                ServiceLocator.MessageSearch,
                                ServiceLocator.ContentSearch,
                                ServiceLocator.ContactSearch,
                                ServiceLocator.ProgramSearch,
                                ServiceLocator.ProgramStore,
                                ServiceLocator.AchievementSearch);

                            if (values.NotificationCompletedLearnerMessageIdentifier.HasValue)
                            {
                                var notification = enrollment.CompletedNotification(organizationId, values.NotificationCompletedLearnerMessageIdentifier.Value, user.UserFirstName, user.UserLastName, values.ProgramName, null);
                                enrollment.SendNotification(notification, user.UserIdentifier);
                            }

                            if (values.NotificationCompletedAdministratorMessageIdentifier.HasValue)
                            {
                                var notification = enrollment.CompletedNotification(organizationId, values.NotificationCompletedAdministratorMessageIdentifier.Value, user.UserFirstName, user.UserLastName, values.ProgramName, null);
                                enrollment.SendNotification(notification, null);
                            }

                            ServiceLocator.ProgramStore.ProgramCompleted(program, user.UserIdentifier, organizationId, values.AchievementIdentifier);
                        }
                    }
                }
            }
        }

        #endregion
    }
}