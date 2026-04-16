using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class StorageData
    {
        /// <summary>
        /// Key ID for the stored MIME
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; private set; }

        /// <summary>
        /// URL for the stored MIME for retrieval, if required
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; private set; }

        /// <summary>
        /// The datacenter region the message is stored in
        /// </summary>
        [JsonProperty(PropertyName = "region")]
        public string Region { get; private set; }

        [JsonConstructor]
        private StorageData()
        {

        }
    }
}