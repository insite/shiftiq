using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Progresses.Write;
using InSite.Persistence;
using InSite.Persistence.Integration.Scorm;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Integration.Scoop;

namespace InSite.Api.Controllers
{
    [DisplayName("Integration")]
    [RoutePrefix("api/integration/scoop")]
    public class ScoopController : ApiOpenController
    {
        [HttpPost]
        [Route("callback/{activityId}/{learnerId}")]
        public HttpResponseMessage Post([FromUri] Guid activityId, [FromUri] Guid learnerId)
        {
            try
            {
                // This controller is an ApiOpenController, so nothing upstream
                // authenticates the caller (ApiValidationFilter returns
                // ApiAuthenticationType.None for that base type). Scoop posts
                // with no credential of its own, so the proof travels in the
                // URL Shift iQ minted at launch. Without this check the two
                // route identifiers were the whole of what a caller needed to
                // complete somebody's lesson, and an activity id is visible in
                // a portal URL.
                var isSigned = IsSignedByShift(activityId, learnerId);

                if (!isSigned)
                {
                    AppSentry.SentryWarning(
                        $"Refused an unsigned Scoop progress callback for activity {activityId} and learner {learnerId}.");

                    return JsonUnauthorized("This callback URL is missing a valid signature.");
                }

                // TEC-1182. The URL above reaches the learner's browser, so it
                // cannot tell Scoop's POST from one the learner sends. The
                // header can: producing it needs the relay secret. It is checked
                // over the body bytes exactly as received, before anything
                // parses them or writes a row, so a refusal leaves no trace
                // beyond the warning.
                var body = Shift.Common.TaskRunner.RunSync(Request.Content.ReadAsByteArrayAsync);

                var check = CheckScoopHeader(body);

                if (check != ScoopCallbackHeaderCheck.Valid)
                {
                    AppSentry.SentryWarning(
                        $"Refused a Scoop progress callback without a valid {ScoopCallbackHeaderSignature.HeaderName} header ({check}) for activity {activityId} and learner {learnerId}.");

                    return JsonUnauthorized($"This callback is missing a valid {ScoopCallbackHeaderSignature.HeaderName} header.");
                }

                if (body == null || body.Length == 0)
                    return JsonBadRequest("Missing payload data.");

                var payload = Encoding.UTF8.GetString(body);

                var cmiCore = TryDeserialize<ScormCmiCore>(payload);

                if (cmiCore != null)
                {
                    ProcessCmiCore(activityId, learnerId, cmiCore);
                }

                var scormEvent = new TScormEvent
                {
                    EventIdentifier = Guid.NewGuid(),
                    EventData = payload,
                    EventWhen = DateTimeOffset.Now,
                    EventSource = "Scoop"
                };

                var store = new ScormStore();

                store.InsertScormEvent(activityId, scormEvent, DateTimeOffset.Now);

                return JsonSuccess(new { message = $"Scorm event received successfully for Shift activity {activityId}" });
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                return JsonError(ex);
            }
        }

        // The signature rides in the query string because Scoop replays the
        // stored URL verbatim and adds no headers of its own.
        private bool IsSignedByShift(Guid activityId, Guid learnerId)
        {
            var secret = ServiceLocator.AppSettings.Engine.Api.Scoop?.Relay?.Secret;

            var signature = Request.GetQueryNameValuePairs()
                .Where(x => StringHelper.Equals(x.Key, ScoopCallbackSignature.ParameterName))
                .Select(x => x.Value)
                .FirstOrDefault();

            return ScoopCallbackSignature.IsValid(activityId, learnerId, signature, secret);
        }

        private ScoopCallbackHeaderCheck CheckScoopHeader(byte[] body)
        {
            var secret = ServiceLocator.AppSettings.Engine.Api.Scoop?.Relay?.Secret;

            string header = null;

            var hasHeader = Request.Headers.TryGetValues(ScoopCallbackHeaderSignature.HeaderName, out var values);
            if (hasHeader)
                header = values.FirstOrDefault();

            return ScoopCallbackHeaderSignature.Verify(header, body, secret, DateTimeOffset.UtcNow);
        }

        // Every way out of here used to be silent, so a completion that changed
        // nothing looked exactly like one that worked: a 200 to Scoop, and no
        // trace on either side. Each is now a Sentry warning naming the reason.
        private void ProcessCmiCore(Guid activityId, Guid userId, ScormCmiCore cmiCore)
        {
            var isCompletion = ScoopCallbackStatus.IsCompletion(
                cmiCore.Status, cmiCore.LessonStatus, cmiCore.CompletionStatus, cmiCore.SuccessStatus);

            // Scoop sends a callback only for a completion, so one that does not
            // read as a completion means the two sides disagree about the body.
            if (!isCompletion)
            {
                AppSentry.SentryWarning(
                    $"A Scoop progress callback for activity {activityId} and learner {userId} carried no completion status Shift iQ recognizes.");
                return;
            }

            var problem = ScormCourseCompleted(activityId, userId, DateTimeOffset.Now);

            if (problem != null)
            {
                AppSentry.SentryWarning(
                    $"A Scoop completion for activity {activityId} and learner {userId} was not recorded: {problem}.");
            }
        }

        // Null when the completion was recorded, otherwise the reason it was not.
        private string ScormCourseCompleted(Guid activityId, Guid userId, DateTimeOffset completed)
        {
            var activity = CourseSearch.SelectActivity(activityId);
            if (activity == null)
                return "the activity does not exist";

            if (activity.GradeItemIdentifier == null)
                return "the activity has no grade item";

            var gradeitem = ServiceLocator.RecordSearch.GetGradeItem(activity.GradeItemIdentifier.Value);
            if (gradeitem == null)
                return "the activity's grade item does not exist";

            return MarkProgressComplete(gradeitem.GradebookIdentifier, gradeitem.GradeItemIdentifier, userId, completed, true);
        }

        // Null when the progress is complete, otherwise the reason it is not.
        private string MarkProgressComplete(Guid gradebook, Guid gradeitem, Guid user, DateTimeOffset? when, bool requireEnrollment)
        {
            if (requireEnrollment)
            {
                var isEnrolled = ServiceLocator.RecordSearch.EnrollmentExists(gradebook, user);

                if (!isEnrolled)
                    return "the learner is not enrolled in the gradebook";
            }

            var progress = ServiceLocator.RecordSearch.GetProgress(gradebook, gradeitem, user);

            Guid id;

            if (progress != null)
            {
                id = progress.ProgressIdentifier;

                if (progress.ProgressIsCompleted && progress.ProgressCompleted == when)
                    return null;
            }
            else
            {
                var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, gradebook, gradeitem, user);

                ServiceLocator.SendCommand(command);

                id = command.AggregateIdentifier;
            }

            ServiceLocator.SendCommand(new CompleteProgress(id, when, null, null, null));

            return null;
        }

        private static T TryDeserialize<T>(string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return null;
            }
        }
    }

    public class ScormCmiCore
    {
        // The status Scoop projects for every standard; read first.
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("cmi.core.lesson_status")]
        public string LessonStatus { get; set; }

        [JsonProperty("cmi.completion_status")]
        public string CompletionStatus { get; set; }

        [JsonProperty("cmi.success_status")]
        public string SuccessStatus { get; set; }

        [JsonProperty("cmi.core.total_time")]
        public string TotalTime { get; set; }
    }
}