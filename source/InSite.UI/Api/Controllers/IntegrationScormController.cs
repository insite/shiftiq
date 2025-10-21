using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Persistence.Integration.Moodle;

namespace InSite.Api.Controllers
{
    [DisplayName("Integration")]
    [RoutePrefix("api/integration/scorm")]
    public class ScormController : ApiOpenController
    {
        [HttpPost]
        [Route("callback/{activityId}")]
        public HttpResponseMessage Post([FromUri] Guid activityId)
        {
            var payload = Shift.Common.TaskRunner.RunSync(Request.Content.ReadAsStringAsync);
            if (payload == null)
                return JsonBadRequest("Missing payload data.");

            var scormEvent = new TScormEvent
            {
                EventIdentifier = Guid.NewGuid(),
                EventData = payload,
                EventWhen = DateTimeOffset.Now
            };

            var store = new ScormStore();
            store.InsertScormEvent(activityId, scormEvent, DateTimeOffset.Now);

            return JsonSuccess(new { message = $"Scorm event received successfully for Shift activity {activityId}" });
        }
    }
}