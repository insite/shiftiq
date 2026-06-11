using Newtonsoft.Json;

namespace Shift.Common.MailgunWebhook
{
    public sealed class Payload
    {
        [JsonProperty(PropertyName = "signature")]
        public PayloadSignature Signature { get; private set; }

        [JsonProperty(PropertyName = "event-data")]
        public EventDataBase EventData { get; private set; }

        [JsonConstructor]
        private Payload()
        {

        }
    }
}