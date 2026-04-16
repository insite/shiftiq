using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;

namespace InSite.Api.Models.MailgunWebhook
{
    [JsonConverter(typeof(JsonEventDataConverter))]
    public abstract class EventDataBase
    {
        /// <summary>
        /// The event name
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public string Name { get; private set; }

        /// <summary>
        /// GUID identifying the individual event
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// Unix epoch, in nanoseconds, when the event was first created
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public double UnixTimestamp { get; private set; }

        [JsonIgnore]
        public DateTime? Timestamp => UnixTimestamp > 0 ? Clock.FromUnixTimestamp(UnixTimestamp) : (DateTime?)null;

        /// <summary>
        /// Logging categorization between (info|warn|error)
        /// </summary>
        [JsonProperty(PropertyName = "log-level")]
        public EventLogLevel LogLevel { get; private set; }

        [JsonProperty(PropertyName = "account")]
        public AccountData Account { get; private set; }

        [JsonProperty(PropertyName = "flags")]
        public EventDataFlags Flags { get; private set; }

        [JsonProperty(PropertyName = "message")]
        public MessageData Message { get; private set; }

        [JsonProperty(PropertyName = "tags")]
        public IReadOnlyList<string> Tags { get; private set; }

        /// <summary>
        /// Variables included in the email
        /// </summary>
        [JsonProperty(PropertyName = "user-variables")]
        public IReadOnlyDictionary<string, string> UserVariables { get; private set; }

        [JsonConstructor]
        protected EventDataBase()
        {

        }

        public bool TryParseEventGuid(out Guid result)
        {
            try
            {
                var base64 = Id.Replace('-', '+').Replace('_', '/') + "==";
                var bytes = Convert.FromBase64String(base64);

                result = new Guid(bytes);

                return true;
            }
            catch
            {
                result = Guid.Empty;
            }

            return false;
        }
    }
}
