using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;

using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class DeliveryStatusData
    {
        /// <summary>
        /// SMTP status code received as a result of the ESP session
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int SmtpStatusCode { get; private set; }

        /// <summary>
        /// The current attempt number trying to deliver the message to the ESP
        /// </summary>
        [JsonProperty(PropertyName = "attempt-no")]
        public int DeliveryAttemptNumber { get; private set; }

        [JsonProperty(PropertyName = "message")]
        public string Message  { get; private set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; private set; }

        /// <summary>
        /// A more specific SMTP error code from the ESP
        /// </summary>
        [JsonProperty(PropertyName = "enhanced-code")]
        public string SmtpEnhancedStatusCode { get; private set; }

        /// <summary>
        /// The mailing host connected to for the SMTP session
        /// </summary>
        [JsonProperty(PropertyName = "mxhost")]
        public string MailingHost { get; private set; }

        [JsonProperty(PropertyName = "certificate-verified")]
        public bool IsCertificateVerified { get; private set; }

        /// <summary>
        /// True if the SMTP session was performed over a TLS connection with the ESP
        /// </summary>
        [JsonProperty(PropertyName = "tls")]
        public bool IsTlsConnection { get; private set; }

        /// <summary>
        /// True if the SMTP session was able to use UTF-8 encoding
        /// </summary>
        [JsonProperty(PropertyName = "utf8")]
        public bool IsUtf8Supported { get; private set; }

        /// <summary>
        /// Time elapsed between when the message is accepted by us and the first delivery attempt to the email service provider (ESP)
        /// </summary>
        [JsonProperty(PropertyName = "first-delivery-attempt-seconds")]
        public int TimeToFirstDeliveryAttempt { get; private set; }

        /// <summary>
        /// The time, in seconds, the SMTP session for this message took
        /// </summary>
        [JsonProperty(PropertyName = "session-seconds")]
        public int SmtpSessionDuration { get; private set; }

        /// <summary>
        /// If the message failed for a reason that can be retried, the number of seconds between retry attempts.This value changes as the number of retries grows!
        /// </summary>
        [JsonProperty(PropertyName = "retry-seconds")]
        public int RetryInterval { get; private set; }

        [JsonConstructor]
        private DeliveryStatusData() 
        { 

        }
    }
}