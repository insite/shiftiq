using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class MessageData
    {
        [JsonProperty(PropertyName = "headers")]
        public MessageDataHeaders Headers { get; private set; }

        [JsonProperty(PropertyName = "attachments")]
        public IReadOnlyList<MessageDataAttachment> Attachments { get; private set; }

        /// <summary>
        /// Total message size, in bytes
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; private set; }

        /// <summary>
        /// Date/Time the message was scheduled for delivery on ingest
        /// </summary>
        [JsonProperty(PropertyName = "scheduled-for")]
        public string ScheduledFor { get; private set; }

        [JsonProperty(PropertyName = "recipients")]
        public IReadOnlyList<string> Recipients { get; private set; }

        [JsonConstructor]
        private MessageData()
        {

        }
    }
}