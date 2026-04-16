using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class MessageDataHeaders
    {
        [JsonProperty(PropertyName = "message-id")]
        public string MessageId { get; private set; }

        /// <summary>
        /// Message FROM header
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public string From { get; private set; }

        /// <summary>
        /// Message TO header
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public string To { get; private set; }

        /// <summary>
        /// Message Subject
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; private set; }

        [JsonConstructor]
        private MessageDataHeaders()
        {

        }
    }
}