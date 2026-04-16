using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class MessageDataAttachment
    {
        /// <summary>
        /// The name of the file attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; private set; }

        /// <summary>
        /// The type of the content attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "content-type")]
        public string ContentType { get; private set; }

        /// <summary>
        /// The attachment size in bytes
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; private set; }

        [JsonConstructor]
        private MessageDataAttachment()
        {

        }
    }
}