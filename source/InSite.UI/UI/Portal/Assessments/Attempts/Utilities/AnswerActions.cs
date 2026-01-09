using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using Shift.Common.Timeline.Exceptions;

using Humanizer;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Write;
using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Portal.Assessments.Attempts.Controls;
using InSite.UI.Portal.Assessments.Attempts.Controls;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using SessionUpdateStatus = InSite.UI.Portal.Assessments.Attempts.Utilities.AttemptSessionInfoCollection.UpdateStatus;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public static class AnswerActions
    {
        #region Methods (helpers)

        private static readonly AttemptSessionInfoCollection _examWindows = new AttemptSessionInfoCollection();

        private static readonly RandomStringGenerator _filePostfixGenerator = new RandomStringGenerator(RandomStringType.Alphanumeric, 8);

        private static bool LoadAnswerData(AnswerLoader loader, out AttemptUrlBase url, out Form bankForm)
        {
            bankForm = null;
            url = loader.IsResource
                ? AttemptUrlResource.Load(AttemptActionType.Ping, loader.ResourceId, loader.AttemptId)
                : loader.IsForm
                    ? (AttemptUrlBase)AttemptUrlForm.Load(AttemptActionType.Ping, loader.FormId, loader.AttemptId)
                    : null;

            if (url == null || !url.AttemptID.HasValue)
            {
                HttpResponseHelper.SendHttp404(false);
                return false;
            }

            if (loader.SessionId.IsEmpty())
            {
                HttpResponseHelper.SendHttp403(false);
                return false;
            }

            var sessionStatus = _examWindows.Update(url.AttemptID.Value, loader.SessionId);
            if (sessionStatus == SessionUpdateStatus.Rejected)
            {
                HttpResponseHelper.SendHttp403(false);
                return false;
            }
            else if (sessionStatus == SessionUpdateStatus.Updated)
            {
                AnswerSectionsContainer.RemoveExcept(url.AttemptID.Value, loader.SessionId);
            }

            bankForm = ServiceLocator.BankSearch.GetFormData(url.FormIdentifier);

            if (IsFormInvalid(bankForm))
            {
                HttpResponseHelper.SendHttp404(false);
                return false;
            }

            return true;
        }

        private static bool IsFormInvalid(Form form)
        {
            return form == null
                || form.Invigilation.Opened.HasValue && form.Invigilation.Opened.Value > DateTimeOffset.Now
                || form.Invigilation.Closed.HasValue && form.Invigilation.Closed.Value < DateTimeOffset.Now;
        }

        private static bool IsAttemptInvalid(Form form, QAttempt attempt)
        {
            var userId = CurrentSessionState.Identity.User.UserIdentifier;

            return attempt == null
                || attempt.FormIdentifier != form.Identifier
                || attempt.LearnerUserIdentifier != userId && attempt.AssessorUserIdentifier != userId
                || !attempt.AttemptStarted.HasValue
                || attempt.AttemptSubmitted.HasValue;
        }

        private static string GetFilePostfix()
        {
            lock (_filePostfixGenerator)
                return _filePostfixGenerator.Next();
        }

        private static (string Result, double Value) GetTimestamp(QAttempt attempt)
        {
            var timerDir = '-';
            var showWarningOnNext = '-';
            var isBreakTime = '-';
            var isEnforcedTimer = '-';
            var isTabInnerTimer = '-';

            var timestamp = -1d;

            if (attempt.AttemptSubmitted.HasValue)
                return (FormatTimestamp(), timestamp);

            timerDir = '1'; // decreasing
            showWarningOnNext = '1';
            isBreakTime = '0';
            isEnforcedTimer = '0';
            isTabInnerTimer = '0';

            var timeLimit = attempt.AttemptTimeLimit ?? 0;
            var duration = ((double?)attempt.AttemptDuration ?? 0).Seconds();

            if (!attempt.IsTimeLimitEnabled)
            {
                timerDir = '0'; // increasing
                timeLimit = -1;
            }
            else if (attempt.SectionsAsTabsEnabled && !attempt.TabNavigationEnabled)
            {
                var section = ServiceLocator.AttemptSearch.GetAttemptSection(attempt.AttemptIdentifier, attempt.ActiveSectionIndex.Value);
                var tabTimeLimit = attempt.TabTimeLimit.ToEnum(SpecificationTabTimeLimit.Disabled);

                if (!section.ShowWarningNextTab)
                    showWarningOnNext = '0';

                if (tabTimeLimit == SpecificationTabTimeLimit.AllTabs)
                {
                    timeLimit = section.TimeLimit.Value;
                    duration = ((double?)section.SectionDuration ?? 0).Seconds();
                    isTabInnerTimer = '1';
                }
                else if (tabTimeLimit == SpecificationTabTimeLimit.SomeTabs && section.IsBreakTimer)
                {
                    isTabInnerTimer = '1';
                    duration = ((double?)section.SectionDuration ?? 0).Seconds();

                    if (section.TimeLimit > 0)
                    {
                        timeLimit = section.TimeLimit.Value;
                    }
                    else
                    {
                        timerDir = '-'; // disabled
                        timeLimit = -1;
                    }
                }

                var isEnforced = section.TimerType.ToEnum(FormSectionTimeType.Optional) == FormSectionTimeType.Enforced;
                if (isEnforced && timeLimit > 0)
                    isEnforcedTimer = '1';

                if (tabTimeLimit != SpecificationTabTimeLimit.Disabled && section.IsBreakTimer)
                    isBreakTime = '1';
            }

            var timePinged = attempt.AttemptPinged ?? attempt.AttemptStarted.Value;

            timestamp = (DateTimeOffset.UtcNow - timePinged + duration).TotalMilliseconds;

            if (timeLimit > 0)
            {
                timestamp = timeLimit * 60000 - timestamp;
                if (timestamp < 0)
                    timestamp = -1d;
            }

            return (FormatTimestamp(), timestamp);

            string FormatTimestamp()
            {
                return $"{isTabInnerTimer}{isEnforcedTimer}{isBreakTime}{showWarningOnNext}{timerDir}.{timestamp:F0}.{Clock.ToUnixMilliseconds(HttpContext.Current.Timestamp):F0}.{Clock.ToUnixMilliseconds(DateTime.UtcNow):F0}";
            }
        }

        public static (bool, string) HandleAction(string action, AnswerLoader loader)
        {
            try
            {
                string result;

                if (action == "post")
                    result = Complete(loader);
                else if (action == "ping")
                    result = Ping(loader);
                else if (action == "submit")
                    result = SubmitAnswer(loader);
                else if (action == "upload")
                    result = UploadFile(loader);
                else if (action == "comment-list")
                    result = GetCommentList(loader);
                else if (action == "comment-get")
                    result = GetComment(loader);
                else if (action == "comment-post")
                    result = PostComment(loader);
                else if (action == "comment-delete")
                    result = DeleteComment(loader);
                else if (action == "next-section")
                    result = GotToNextSection(loader);
                else if (action == "next-question")
                    result = GoToNextQuestion(loader);
                else
                    return (false, null);

                if (result == null)
                    RemoveSectionsData();

                return (true, result);
            }
            catch (ApplicationError apperr)
            {
                RemoveSectionsData();

                if (apperr.Message == AnswerSectionsContainer.ErrorSectionStorageEmpty)
                    return (true, null);

                throw;
            }
            catch (UnhandledCommandException ucex)
            {
                if (ucex.InnerException is ApplicationError apperr && apperr.Message == AttemptAggregate.ErrorPingOutdated)
                    return (true, null);

                throw;
            }
            catch (Exception)
            {
                RemoveSectionsData();
                throw;
            }

            void RemoveSectionsData()
            {
                var url = loader.Url;
                if (url?.AttemptID == null)
                    return;

                if (loader.SessionId.IsNotEmpty())
                    AnswerSectionsContainer.Remove(url.AttemptID.Value, loader.SessionId);
                else
                    AnswerSectionsContainer.Remove(url.AttemptID.Value);
            }
        }

        #endregion

        #region Actions (Complete)

        public static string Complete(AnswerLoader loader)
        {
            Form form;
            AttemptUrlBase baseUrl;
            QAttempt attempt = null;
            AttemptHelper.IAction result;
            Guid?  whenCompleted;

            if (loader.IsForm)
            {
                var url = AttemptUrlForm.Load(AttemptActionType.Answer, loader.FormId, loader.AttemptId);

                result = AttemptHelper.LoadForm(url, out form);

                baseUrl = url;
            }
            else
            {
                var url = AttemptUrlResource.Load(AttemptActionType.Answer, loader.ResourceId, loader.AttemptId);

                result = AttemptHelper.LoadResource(url.PageIdentifier, out form, out _);

                baseUrl = url;
            }

            if (result == null)
                result = AttemptHelper.LoadAttemptAnswer(form, baseUrl, out attempt);

            AnswerSectionsContainer.Remove(baseUrl.AttemptID.Value);

            if (result != null)
            {
                return result is AttemptHelper.ActionRedirect redirect && redirect.Url.IsNotEmpty()
                    ? redirect.Url
                    : "ERROR";
            }

            if (form.AssetVersion > 0)
            {
                AttemptHelper.CompleteAttempt(attempt);
                whenCompleted = form.WhenAttemptCompletedNotifyAdminMessageIdentifier;
            }
            else
            {
                ServiceLocator.SendCommand(new VoidAttempt(attempt.AttemptIdentifier, "Pre-Published Form"));
                ServiceLocator.SendCommand(new AnalyzeForm(form.Specification.Bank.Identifier, form.Identifier));
                whenCompleted = form.WhenAttemptCompletedNotifyAdminMessageIdentifier;
            }

            if (whenCompleted.HasValue)
            {
                AttemptHelper.LoadAttemptResult(form, baseUrl, out var attemptModel);

                AttemptHelper.SendBankNotification(
                    CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                    form,
                    whenCompleted.Value,
                    attemptModel.LearnerUserIdentifier,
                    attemptModel.AttemptScore);
            }

            return baseUrl.GetResultUrl();
        }

        #endregion

        #region Actions (Ping)

        public static string Ping(AnswerLoader loader)
        {
            if (LoadAnswerData(loader, out var url, out var bankForm))
                return Ping(bankForm, url.AttemptID.Value);

            return null;
        }

        private static string Ping(Form bankForm, Guid attemptId)
        {
            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (IsAttemptInvalid(bankForm, attempt))
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var (result, msTimestamp) = GetTimestamp(attempt);

            if (msTimestamp >= 0)
                ServiceLocator.SendCommand(new PingAttempt(attempt.AttemptIdentifier));

            return result;
        }

        #endregion

        #region Actions (Submit Answer)

        private delegate bool FormSubmitHandler(Guid attemptId, QAttemptQuestion question, string value);
        private delegate bool FileSubmitHandler(Guid attemptId, QAttemptQuestion question, HttpPostedFile value);

        private static readonly IReadOnlyDictionary<QuestionItemType, FormSubmitHandler> _formSubmitHandlers = new ReadOnlyDictionary<QuestionItemType, FormSubmitHandler>(new Dictionary<QuestionItemType, FormSubmitHandler>
        {
            { QuestionItemType.SingleCorrect, SubmitAnswerSingleCorrect },
            { QuestionItemType.MultipleCorrect, SubmitAnswerMultipleCorrect },
            { QuestionItemType.ComposedEssay, SubmitAnswerComposedEssay },
            { QuestionItemType.ComposedVoice, SubmitAnswerComposedVoice },
            { QuestionItemType.TrueOrFalse, SubmitAnswerTrueOrFalse },
            { QuestionItemType.BooleanTable, SubmitAnswerBooleanTable },
            { QuestionItemType.Matching, SubmitAnswerMatching },
            { QuestionItemType.HotspotStandard, SubmitAnswerHotspot },
            { QuestionItemType.HotspotImageCaptcha, SubmitAnswerHotspot },
            { QuestionItemType.HotspotMultipleChoice, SubmitAnswerHotspot },
            { QuestionItemType.HotspotMultipleAnswer, SubmitAnswerHotspot },
            { QuestionItemType.HotspotCustom, SubmitAnswerHotspot },
            { QuestionItemType.Ordering, SubmitAnswerOrdering },
        });
        private static readonly IReadOnlyDictionary<QuestionItemType, FileSubmitHandler> _fileSubmitHandlers = new ReadOnlyDictionary<QuestionItemType, FileSubmitHandler>(new Dictionary<QuestionItemType, FileSubmitHandler>
        {
            { QuestionItemType.ComposedVoice, SubmitComposedVoice }
        });

        public static string SubmitAnswer(AnswerLoader loader)
        {
            if (LoadAnswerData(loader, out var url, out var bankForm))
                return SubmitAnswer(bankForm, url.AttemptID.Value, loader.Request);

            return null;
        }

        private static string SubmitAnswer(Form bankForm, Guid attemptId, HttpRequest request)
        {
            if (IsFormInvalid(bankForm))
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (IsAttemptInvalid(bankForm, attempt))
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var (result, msTimestamp) = GetTimestamp(attempt);

            if (msTimestamp >= 0)
                SubmitAnswers(attempt, request);

            ServiceLocator.SendCommand(new PingAttempt(attempt.AttemptIdentifier));

            return result;
        }

        private static void SubmitAnswers(QAttempt attempt, HttpRequest request)
        {
            SubmitFormAnswers(attempt, request);
            SubmitFileAnswers(attempt, request);
        }

        private static void SubmitFormAnswers(QAttempt attempt, HttpRequest request)
        {
            var fields = new List<(string Name, int QuestionSequence, string[] Values)>();

            foreach (string name in request.Form)
            {
                if (!TryGetSubmitQuestionSequence(name, out var sequence))
                    continue;

                var values = request.Form.GetValues(name);
                if (values.Length > 1)
                    ThrowError(name, values);

                fields.Add((name, sequence, values));
            }

            var sequences = fields.Select(x => x.QuestionSequence).ToArray();
            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestionsBySequence(attempt.AttemptIdentifier, sequences);

            foreach (var field in fields)
            {
                var question = questions.SingleOrDefault(x => x.QuestionSequence == field.QuestionSequence);
                if (question == null)
                    ThrowError(field.Name, field.Values);

                var qType = question.QuestionType.ToEnum<QuestionItemType>();
                var isSubmitted = _formSubmitHandlers[qType](attempt.AttemptIdentifier, question, field.Values[0]);

                if (!isSubmitted)
                    ThrowError(field.Name, field.Values);
            }

            void ThrowError(string name, IEnumerable<string> values)
            {
                throw ApplicationError.Create(
                    $"Unexpected Selection Value: {name}={string.Join(",", values)}");
            }
        }

        private static void SubmitFileAnswers(QAttempt attempt, HttpRequest request)
        {
            var fields = new List<(string Name, int QuestionSequence, IList<HttpPostedFile> Files)>();

            foreach (string name in request.Files)
            {
                var files = request.Files.GetMultiple(name);
                if (files.Count == 0)
                    continue;

                if (files.Count > 1 || !TryGetSubmitQuestionSequence(name, out var sequence))
                    ThrowError(name, files);

                fields.Add((name, sequence, files));
            }

            var sequences = fields.Select(x => x.QuestionSequence).ToArray();
            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestionsBySequence(attempt.AttemptIdentifier, sequences);

            foreach (var field in fields)
            {
                var question = questions.SingleOrDefault(x => x.QuestionSequence == field.QuestionSequence);
                if (question == null)
                    ThrowError(field.Name, field.Files);

                var qType = question.QuestionType.ToEnum<QuestionItemType>();
                var isSubmitted = _fileSubmitHandlers[qType](attempt.AttemptIdentifier, question, field.Files[0]);

                if (!isSubmitted)
                    ThrowError(field.Name, field.Files);
            }

            void ThrowError(string name, IEnumerable<HttpPostedFile> files)
            {
                throw ApplicationError.Create(
                    $"Unexpected Selection Value: {name}=({string.Join("),(", files.Select(x => $"name:{x.FileName},type:{x.ContentType},size:{x.ContentLength}"))}");
            }
        }

        private static bool TryGetSubmitQuestionSequence(string name, out int sequence)
        {
            sequence = -1;

            return name.StartsWith("q_") && int.TryParse(name.Substring(2), out sequence);
        }

        private static bool SubmitAnswerSingleCorrect(Guid attemptId, QAttemptQuestion question, string value)
        {
            if (value.IsEmpty())
                return true;

            var questionId = question.QuestionIdentifier;
            var optionSequence = int.Parse(value);
            var optionKey = ServiceLocator.AttemptSearch.GetAttemptOptionKeyBySequence(attemptId, questionId, optionSequence);

            if (!optionKey.HasValue)
                return false;

            ServiceLocator.SendCommand(new AnswerMultipleChoiceQuestion(attemptId, questionId, optionKey.Value));

            return true;
        }

        private static bool SubmitAnswerMultipleCorrect(Guid attemptId, QAttemptQuestion question, string value)
        {
            var optionSequences = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            if (optionSequences.Length == 0)
                return false;

            var questionId = question.QuestionIdentifier;
            var optionKeys = ServiceLocator.AttemptSearch.GetAttemptOptionKeysBySequence(attemptId, questionId, optionSequences);
            var isValid = optionKeys.Count == optionSequences.Length;

            if (isValid)
                ServiceLocator.SendCommand(new AnswerMultipleCorrectQuestion(attemptId, question.QuestionIdentifier, optionKeys.ToArray()));

            return isValid;
        }

        private static bool SubmitAnswerComposedEssay(Guid attemptId, QAttemptQuestion question, string value)
        {
            ServiceLocator.SendCommand(new AnswerComposedQuestion(attemptId, question.QuestionIdentifier, value));

            return true;
        }

        private static bool SubmitAnswerComposedVoice(Guid attemptId, QAttemptQuestion question, string value)
        {
            if (value == "start")
            {
                ServiceLocator.SendCommand(new StartComposedQuestionAttempt(attemptId, question.QuestionIdentifier));
                return true;
            }

            return false;
        }

        private static bool SubmitComposedVoice(Guid attemptId, QAttemptQuestion question, HttpPostedFile value)
        {
            if (question.AnswerAttemptLimit > 0)
            {
                if (question.AnswerSubmitAttempt >= question.AnswerAttemptLimit)
                    throw ApplicationError.Create("The limit of composed voice question attempts has been exceeded");

                if (question.AnswerSubmitAttempt >= question.AnswerRequestAttempt)
                    throw ApplicationError.Create("There is no request to start attempting the composed voice question");
            }

            var capture = InputAudio.GetAudioCapture(value, AnswerQuestionOutput.ComposedVoiceBitrate, question.AnswerTimeLimit ?? 0);
            if (!capture.IsValid)
                throw ApplicationError.Create(capture.ValidationError.IfNullOrEmpty("Unknown Error"));

            var previousFileId = question.AnswerFileIdentifier;
            var fileModel = capture.Save(attemptId, FileObjectType.Attempt);

            ServiceLocator.SendCommand(new AnswerComposedQuestion(attemptId, question.QuestionIdentifier, fileModel.FileIdentifier.ToString()));

            if (previousFileId.HasValue)
                ServiceLocator.StorageService.Delete(previousFileId.Value);

            return true;
        }

        private static bool SubmitAnswerTrueOrFalse(Guid attemptId, QAttemptQuestion question, string value)
        {
            if (value.IsEmpty())
                return true;

            var questionId = question.QuestionIdentifier;
            var optionSequence = int.Parse(value);
            var optionKey = ServiceLocator.AttemptSearch.GetAttemptOptionKeyBySequence(attemptId, questionId, optionSequence);

            if (!optionKey.HasValue)
                return false;

            ServiceLocator.SendCommand(new AnswerTrueOrFalseQuestion(attemptId, question.QuestionIdentifier, optionKey.Value));

            return true;
        }

        private static bool SubmitAnswerBooleanTable(Guid attemptId, QAttemptQuestion question, string value)
        {
            if (value.IsEmpty())
                return true;

            var isValid = true;
            var selectedOptions = new Dictionary<int, bool>();
            var questionOptions = ServiceLocator.AttemptSearch.GetAttemptOptions(attemptId, question.QuestionIdentifier)
                .ToDictionary(x => x.OptionSequence, x => x.OptionKey);

            foreach (var o in value.Split(new[] { ',' }))
            {
                var parts = o.Split(new[] { ':' });
                if (parts.Length != 2)
                {
                    isValid = false;
                    break;
                }

                var sequence = int.Parse(parts[0]);
                var selected = parts[1] == "1" ? true : parts[1] == "0" ? false : (bool?)null;

                if (!questionOptions.ContainsKey(sequence) || !selected.HasValue)
                {
                    isValid = false;
                    break;
                }

                var key = questionOptions[sequence];
                if (selectedOptions.ContainsKey(key))
                {
                    isValid = false;
                    break;
                }

                selectedOptions.Add(key, selected.Value);
            }

            if (isValid = isValid && selectedOptions.Count > 0)
                ServiceLocator.SendCommand(new AnswerBooleanTableQuestion(attemptId, question.QuestionIdentifier, selectedOptions));

            return isValid;
        }

        private static bool SubmitAnswerMatching(Guid attemptId, QAttemptQuestion question, string value)
        {
            var isValid = true;
            var matchDistractors = question.GetMatchDistractors();
            var questionMatches = ServiceLocator.AttemptSearch.GetAttemptMatches(attemptId, question.QuestionIdentifier);
            var valueArray = JsonConvert.DeserializeObject<string[]>(value) ?? new string[0];

            var matchAnswers = new Dictionary<int, string>();
            var validSequences = new HashSet<int>(questionMatches.Select(x => x.MatchSequence));
            var validMatches = new HashSet<string>(questionMatches.Select(x => x.MatchRightText).Concat(matchDistractors));

            foreach (var v in valueArray)
            {
                var index = v == null ? -1 : v.IndexOf(':');
                if (index <= 0)
                {
                    isValid = false;
                    break;
                }

                var sequence = int.Parse(v.Substring(0, index));
                var text = v.Substring(index + 1);

                if (!validSequences.Contains(sequence) || text.IsNotEmpty() && !validMatches.Contains(text))
                {
                    isValid = false;
                    break;
                }

                matchAnswers.Add(sequence, text);
            }

            if (isValid)
                ServiceLocator.SendCommand(new AnswerMatchingQuestion(attemptId, question.QuestionIdentifier, matchAnswers));

            return isValid;
        }

        private static bool SubmitAnswerHotspot(Guid attemptId, QAttemptQuestion question, string value)
        {
            if (value.IsEmpty() || value == "-1")
                return true;

            var pins = new List<AttemptHotspotPinAnswer>();
            var items = value.Split(';');

            foreach (var item in items)
            {
                var parts = item.Split(',');
                if (parts.Length != 2)
                    return false;

                if (!decimal.TryParse(parts[0], out var x))
                    return false;

                if (!decimal.TryParse(parts[1], out var y))
                    return false;

                if (x == -1 && y == -1)
                    return true;

                if (x < 0 || x > 1 || y < 0 || y > 1)
                    return false;

                pins.Add(new AttemptHotspotPinAnswer(x, y));
            }

            if (pins.Count == 0)
                return false;

            ServiceLocator.SendCommand(new AnswerHotspotQuestion(attemptId, question.QuestionIdentifier, pins.ToArray()));

            return true;
        }

        private static bool SubmitAnswerOrdering(Guid attemptId, QAttemptQuestion question, string value)
        {
            if (value.IsEmpty())
                return true;

            var ids = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var options = ServiceLocator.AttemptSearch.GetAttemptOptions(attemptId, question.QuestionIdentifier);
            if (ids.Length != options.Count)
                return false;

            var order = new int[ids.Length];

            for (var i = 0; i < ids.Length; i++)
            {
                var option = ParseId(ids[i], out var qSequence, out var oSequence)
                    ? options.FirstOrDefault(x => x.QuestionSequence == qSequence && x.OptionSequence == oSequence)
                    : null;

                if (option == null)
                    return false;

                order[i] = option.OptionKey;
            }

            ServiceLocator.SendCommand(new AnswerOrderingQuestion(attemptId, question.QuestionIdentifier, order.ToArray()));

            return true;

            bool ParseId(string id, out int qSequence, out int oSequence)
            {
                qSequence = default;
                oSequence = default;

                var parts = id.Split('_');
                if (parts.Length != 2)
                    return false;

                qSequence = int.Parse(parts[0]);
                oSequence = int.Parse(parts[1]);

                return true;
            }
        }

        #endregion

        #region Actions (Upload File)

        [JsonObject(MemberSerialization.OptIn)]
        private class JsonUploadResult
        {
            public JsonUploadResult()
            {
                Type = "UPLOADED";
                Links = new List<string>();
                Errors = new List<string>();
            }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; }

            [JsonProperty(PropertyName = "time")]
            public string Timestamp { get; set; }

            [JsonProperty(PropertyName = "links")]
            public List<string> Links { get; }

            [JsonProperty(PropertyName = "errors")]
            public List<string> Errors { get; }
        }

        public static string UploadFile(AnswerLoader loader)
        {
            if (LoadAnswerData(loader, out var url, out var bankForm))
                return UploadFile(bankForm, url.AttemptID.Value, loader.Request);

            return null;
        }

        private static string UploadFile(Form bankForm, Guid attemptId, HttpRequest request)
        {
            const int maxFileSize = 2 * 1024 * 1024;

            if (request.Files.Count == 0)
            {
                HttpResponseHelper.SendHttp400(false);
                return null;
            }

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (IsAttemptInvalid(bankForm, attempt))
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var timestamp = GetTimestamp(attempt);

            var organizationId = CurrentSessionState.Identity.Organization.OrganizationIdentifier;
            var result = new JsonUploadResult
            {
                Timestamp = timestamp.Result
            };

            if (timestamp.Value >= 0)
            {
                var files = request.Files.AllKeys.Select(key =>
                {
                    var file = request.Files[key];
                    var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    var fileInfo = new
                    {
                        Name = fileName,
                        Extension = fileExtension,
                        Length = file.ContentLength,
                        IsImage = fileExtension.IsNotEmpty() && FileExtension.IsImage(fileExtension),
                        IsDocument = fileExtension.IsNotEmpty() && FileExtension.IsDocument(fileExtension),
                        IsArchive = fileExtension.IsNotEmpty() && FileExtension.IsArchive(fileExtension),
                        Stream = file.InputStream
                    };

                    if (fileInfo.Length == 0)
                        result.Errors.Add("The uploaded file is empty.");
                    else if (fileInfo.Length > maxFileSize)
                        result.Errors.Add($"You cannot upload a file larger than {maxFileSize.Bytes().Humanize("0.##")}.");
                    else if (!fileInfo.IsImage && !fileInfo.IsDocument && !fileInfo.IsArchive)
                        result.Errors.Add("The uploaded file has invalid file extension.");

                    return fileInfo;
                }).ToArray();

                if (result.Errors.Count == 0)
                {
                    foreach (var info in files)
                    {
                        var path = $"/attempts/{attemptId}/{info.Name}_{GetFilePostfix()}{info.Extension}";
                        var file = Common.Web.Infrastructure.FileHelper.Provider.Save(organizationId, path, info.Stream, isCheckFileSizeLimits: false);
                        var url = Common.Web.Infrastructure.FileHelper.GetUrl(file.Path);

                        result.Links.Add($"{(info.IsImage ? "!" : string.Empty)}[{info.Name}{info.Extension}]({url})");
                    }
                }
            }

            ServiceLocator.SendCommand(new PingAttempt(attempt.AttemptIdentifier));

            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region Actions (Comments)

        public static string GetCommentList(AnswerLoader loader)
        {
            var attemptId = Guid.Parse(loader.Request.Form["attemptId"]);

            var control = (CommentList)loader.LoadControl("~/UI/Portal/Assessments/Attempts/Controls/CommentList.ascx");
            control.LoadData(attemptId);

            var content = new StringBuilder();
            using (var stringWriter = new StringWriter(content))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    control.RenderControl(htmlWriter);
            }

            return content.ToString();
        }

        public static string GetComment(AnswerLoader loader)
        {
            if (!LoadCommentData(loader, out var examAttempt, out var attemptQuestion))
                return null;

            var comment = ServiceLocator.AttemptSearch.GetQAttemptComment(examAttempt.AttemptIdentifier, attemptQuestion.QuestionIdentifier, CurrentSessionState.Identity.User.UserIdentifier);

            return comment?.CommentText ?? string.Empty;
        }

        public static string PostComment(AnswerLoader loader)
        {
            if (!LoadCommentData(loader, out var examAttempt, out var attemptQuestion))
                return null;

            var text = loader.Request.Form["text"];

            if (text.IsEmpty())
                text = "No comment.";

            ServiceLocator.SendCommand(new AuthorComment(examAttempt.AttemptIdentifier, attemptQuestion.QuestionIdentifier, text));

            return "OK";
        }

        public static string DeleteComment(AnswerLoader loader)
        {
            if (!LoadCommentData(loader, out var examAttempt, out var attemptQuestion))
                return null;

            ServiceLocator.SendCommand(new AuthorComment(examAttempt.AttemptIdentifier, attemptQuestion.QuestionIdentifier, null));

            return "OK";
        }

        private static bool LoadCommentData(AnswerLoader loader, out QAttempt attempt, out QAttemptQuestion question)
        {
            var attemptId = Guid.Parse(loader.Request.Form["attemptId"]);
            var questionSequence = int.Parse(loader.Request.Form["question"]);
            var userId = CurrentSessionState.Identity.User.UserIdentifier;

            attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            question = attempt != null && (attempt.LearnerUserIdentifier == userId || attempt.AssessorUserIdentifier == userId)
                ? ServiceLocator.AttemptSearch.GetAttemptQuestion(attempt.AttemptIdentifier, questionSequence)
                : null;

            if (question == null)
            {
                HttpResponseHelper.SendHttp404(false);
                return false;
            }

            return true;
        }

        #endregion

        #region Actions (Next Section/Question)

        public static string GotToNextSection(AnswerLoader loader)
        {
            if (LoadAnswerData(loader, out var url, out var bankForm))
                return GotToNextSection(bankForm, url.AttemptID.Value, loader.SessionId, loader.Request);

            return null;
        }

        private static string GotToNextSection(Form bankForm, Guid attemptId, string sessionId, HttpRequest request)
        {
            var sections = AnswerSectionsContainer.Load(attemptId, sessionId);
            if (sections == null)
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var index = int.Parse(request.Form["index"]);
            ServiceLocator.SendCommand(new SwitchAttemptSection(attemptId, index));

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (attempt == null || !attempt.ActiveSectionIndex.HasValue || attempt.ActiveSectionIndex.Value != index)
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var section = sections.Get(index);
            var (timestamp, _) = GetTimestamp(attempt);

            return timestamp + "|"
                + section.NavItemIndex.ToString() + "|"
                + section.Html;
        }

        public static string GoToNextQuestion(AnswerLoader loader)
        {
            if (LoadAnswerData(loader, out var url, out var bankForm))
                return GoToNextQuestion(bankForm, url.AttemptID.Value, loader.SessionId, loader.Request);

            return null;
        }

        private static string GoToNextQuestion(Form bankForm, Guid attemptId, string sessionId, HttpRequest request)
        {
            var sections = AnswerSectionsContainer.Load(attemptId, sessionId);
            if (sections == null)
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var questionIndex = int.Parse(request.Form["index"]);
            ServiceLocator.SendCommand(new SwitchAttemptQuestion(attemptId, questionIndex));

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
            if (attempt == null || !attempt.ActiveSectionIndex.HasValue || !attempt.ActiveQuestionIndex.HasValue)
            {
                HttpResponseHelper.SendHttp404(false);
                return null;
            }

            var section = sections.Get(attempt.ActiveSectionIndex.Value, attempt.ActiveQuestionIndex.Value);
            var (timestamp, _) = GetTimestamp(attempt);

            return timestamp + "|"
                + section.NavItemIndex.ToString() + "|"
                + attempt.ActiveQuestionIndex.Value.ToString() + "|"
                + section.Html;
        }
        #endregion
    }
}