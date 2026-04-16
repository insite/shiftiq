using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class EventDataComplained : EventDataBase
    {
        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; private set; }

        [JsonProperty(PropertyName = "envelope")]
        public EnvelopeData Envelope { get; private set; }

        [JsonConstructor]
        private EventDataComplained()
        {

        }
    }
}