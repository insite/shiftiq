using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Domain.Integrations.Scorm;
using InSite.Persistence.Integration.Moodle;

namespace InSite.Api.Controllers
{
    [DisplayName("Integration")]
    [RoutePrefix("api/integration/moodle")]
    public class MoodleController : ApiOpenController
    {
        [HttpPost]
        [Route("callback/{activityId}")]
        public HttpResponseMessage Post([FromUri] Guid activityId, [FromBody] MoodleEvent moodleEvent)
        {
            if (moodleEvent == null)
                return JsonBadRequest("Missing payload data.");

            var store = new MoodleStore();
            store.InsertMoodleEvent(activityId, moodleEvent, DateTimeOffset.Now);

            return JsonSuccess(new { message = $"Moodle event received successfully for Shift activity {activityId}" });
        }
    }
}