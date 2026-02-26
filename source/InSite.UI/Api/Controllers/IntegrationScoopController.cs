using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Progresses.Write;
using InSite.Persistence;
using InSite.Persistence.Integration.Moodle;

using Newtonsoft.Json;

using Shift.Common;

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
                var payload = Shift.Common.TaskRunner.RunSync(Request.Content.ReadAsStringAsync);

                if (payload == null)
                    return JsonBadRequest("Missing payload data.");

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

        private void ProcessCmiCore(Guid activityId, Guid userId, ScormCmiCore cmiCore)
        {
            if (StringHelper.EqualsAny(cmiCore.LessonStatus, new[] { "completed", "passed" }))
                ScormCourseCompleted(activityId, userId, DateTimeOffset.Now);
        }

        private bool ScormCourseCompleted(Guid activityId, Guid userId, DateTimeOffset completed)
        {
            var activity = CourseSearch.SelectActivity(activityId);
            var gradeItem = activity?.GradeItemIdentifier;
            if (gradeItem == null)
                return false;

            var gradeitem = ServiceLocator.RecordSearch.GetGradeItem(activity.GradeItemIdentifier.Value);
            if (gradeitem == null)
                return false;

            return Complete(gradeitem.GradebookIdentifier, gradeitem.GradeItemIdentifier, userId, completed, true);
        }

        private bool Complete(Guid gradebook, Guid gradeitem, Guid user, DateTimeOffset? when, bool requireEnrollment)
        {
            var progress = ServiceLocator.RecordSearch.GetProgress(gradebook, gradeitem, user);
            var isEnrolled = ServiceLocator.RecordSearch.EnrollmentExists(gradebook, user);

            if (requireEnrollment && !isEnrolled)
                return false;

            Guid id;

            if (progress != null)
            {
                id = progress.ProgressIdentifier;
            }
            else
            {
                var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, gradebook, gradeitem, user);

                ServiceLocator.SendCommand(command);

                id = command.AggregateIdentifier;
            }

            if (!progress.ProgressIsCompleted || progress.ProgressCompleted != when)
            {
                ServiceLocator.SendCommand(new CompleteProgress(id, when, null, null, null));
                return true;
            }

            return false;
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
        [JsonProperty("cmi.core.lesson_status")]
        public string LessonStatus { get; set; }

        [JsonProperty("cmi.core.total_time")]
        public string TotalTime { get; set; }
    }
}