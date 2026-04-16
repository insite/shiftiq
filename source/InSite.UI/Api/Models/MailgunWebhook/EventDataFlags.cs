using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class EventDataFlags
    {
        /// <summary>
        /// true if it’s an outgoing message. false if it’s incoming.
        /// </summary>
        [JsonProperty(PropertyName = "is-authenticated")]
        public bool? IsAuthenticated { get; private set; }

        /// <summary>
        /// true if the message was sent as a result of a Route match
        /// </summary>
        [JsonProperty(PropertyName = "is-routed")]
        public bool? IsRouted { get; private set; }

        /// <summary>
        /// Tells if the message has AMP component in
        /// </summary>
        [JsonProperty(PropertyName = "is-amp")]
        public bool? IsAmp { get; private set; }

        /// <summary>
        /// Tells if the message has been encrypted before stored
        /// </summary>
        [JsonProperty(PropertyName = "is-encrypted")]
        public bool? IsEncrypted { get; private set; }

        /// <summary>
        /// If true, the message has been marked as delivered but the actual send stop before sending to the ESP
        /// </summary>
        [JsonProperty(PropertyName = "is-test-mode")]
        public bool? IsTestMode { get; private set; }

        [JsonConstructor]
        private EventDataFlags()
        {

        }
    }
}