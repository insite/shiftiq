using InSite.Common.Web.UI;
using InSite.UI.Admin.Integrations.Tests.Utilities;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Integrations.Tests.Controls
{
    public partial class D365CancelEvent : BaseUserControl, ID365Method
    {
        public void InitMethod()
        {
            EventIdentifier.Filter.EventType = "Exam";
        }

        public string GetUrl()
        {
            var eventId = EventIdentifier.Value;

            return eventId.HasValue ? $"/api/events/{eventId.Value}/cancel" : null;
        }

        public string GetBody(Formatting jsonFormatting)
        {
            var data = new CancelEvent
            {
                Reason = Reason.Text
            };

            return JsonConvert.SerializeObject(data, jsonFormatting);
        }

        public D365Response SendRequest()
        {
            return D365Response.Get(GetUrl(), "POST", "application/json", GetBody(Formatting.None));
        }
    }
}