using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class EnvelopeData
    {
        /// <summary>
        /// The sender address
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        public string Sender { get; private set; }

        /// <summary>
        /// The recipient address
        /// </summary>
        [JsonProperty(PropertyName = "targets")]
        public string Recipient { get; private set; }

        /// <summary>
        /// The protocol used to make the send.Either http or smtp
        /// </summary>
        [JsonProperty(PropertyName = "transport")]
        public string TransportProtocol { get; private set; }

        /// <summary>
        /// The Mailgun IP the email has been sent from
        /// </summary>
        [JsonProperty(PropertyName = "sending-ip")]
        public string SendingIpAddress { get; private set; }

        [JsonConstructor]
        private EnvelopeData()
        {

        }
    }
}