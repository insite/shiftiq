using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class ClientInfo
    {
        /// <summary>
        /// Categorize client between: mobile browser, library, email client, robot, feed reader or other
        /// </summary>
        [JsonProperty(PropertyName = "client-type")]
        public string ClientType { get; private set; }

        /// <summary>
        /// The client Operating System
        /// </summary>
        [JsonProperty(PropertyName = "client-os")]
        public string ClientOs { get; private set; }

        /// <summary>
        /// Could be: desktop, mobile, table or unknown
        /// </summary>
        [JsonProperty(PropertyName = "device-type")]
        public string DeviceType { get; private set; }

        /// <summary>
        /// The client product identifier
        /// </summary>
        [JsonProperty(PropertyName = "client-name")]
        public string ClientName { get; private set; }

        [JsonProperty(PropertyName = "user-agent")]
        public string UserAgent { get; private set; }

        [JsonConstructor]
        private ClientInfo()
        {

        }
    }
}