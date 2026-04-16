using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class EventDataAccepted : EventDataBase
    {
        [JsonProperty(PropertyName = "method")]
        public string Method { get; private set; }

        [JsonProperty(PropertyName = "envelope")]
        public EnvelopeData Envelope { get; private set; }

        [JsonProperty(PropertyName = "storage")]
        public StorageData Storage { get; private set; }

        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; private set; }

        /// <summary>
        /// ESP domain
        /// </summary>
        [JsonProperty(PropertyName = "recipient-domain")]
        public string RecipientDomain { get; private set; }

        [JsonConstructor]
        private EventDataAccepted()
        {

        }
    }
}