using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class EventDataOpened : EventDataBase
    {
        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; private set; }

        /// <summary>
        /// Name of the Inbox Provider for the given recipient, if known
        /// </summary>
        [JsonProperty(PropertyName = "recipient-provider")]
        public string RecipientProvider { get; private set; }

        [JsonProperty(PropertyName = "ip")]
        public string IpAddress { get; private set; }

        /// <summary>
        /// Location data based on the client IP
        /// </summary>
        [JsonProperty(PropertyName = "geolocation")]
        public GeolocationData Geolocation { get; private set; }

        [JsonProperty(PropertyName = "client-info")]
        public ClientInfo ClientInfo { get; private set; }

        [JsonConstructor]
        private EventDataOpened()
        {

        }
    }
}