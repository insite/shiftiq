using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ExpirationInfo
    {
        private static readonly DateTime _timestampStart = new DateTime(2021, 12, 12, 0, 0, 0, DateTimeKind.Utc);

        [JsonProperty(PropertyName = "id")]
        public long Timestamp { get; }

        [JsonProperty(PropertyName = "t")]
        public int Timeout { get; }

        public ExpirationInfo(DateTime expireDate)
        {
            Timestamp = (long)(expireDate - _timestampStart).TotalSeconds;
            Timeout = (int)(expireDate - DateTime.UtcNow).TotalSeconds;
        }
    }
}