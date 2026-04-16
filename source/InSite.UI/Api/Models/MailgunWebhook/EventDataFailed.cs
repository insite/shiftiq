using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class EventDataFailed : EventDataBase
    {
        /// <summary>
        /// Filter by event severity, if exists.
        /// </summary>
        [JsonProperty(PropertyName = "severity")]
        public string Severity { get; private set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; private set; }

        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; private set; }

        /// <summary>
        /// ESP domain
        /// </summary>
        [JsonProperty(PropertyName = "recipient-domain")]
        public string RecipientDomain { get; private set; }

        /// <summary>
        /// Name of the Inbox Provider for the given recipient, if known
        /// </summary>
        [JsonProperty(PropertyName = "recipient-provider")]
        public string RecipientProvider { get; private set; }

        [JsonProperty(PropertyName = "envelope")]
        public EnvelopeData Envelope { get; private set; }

        [JsonProperty(PropertyName = "delivery-status")]
        public DeliveryStatusData DeliveryStatus { get; private set; }

        [JsonProperty(PropertyName = "storage")]
        public StorageData Storage { get; private set; }

        [JsonIgnore]
        public bool IsTemporary => Name == "temporary_fail" || Severity == "temporary";

        [JsonConstructor]
        private EventDataFailed()
        {

        }
    }
}