using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ReCaptcha
    {
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; private set; }

        [JsonProperty(PropertyName = "challenge_ts")]
        public DateTime Timestamp { get; private set; }

        [JsonProperty(PropertyName = "hostname")]
        public string HostName { get; private set; }

        [JsonProperty(PropertyName = "error-codes")]
        public string[] ErrorCodes { get; private set; }

        public static ReCaptcha Validate(
            Guid? userIdentifier,
            Guid organizationIdentifier,
            string secret,
            string response,
            string remoteIp,
            IApiRequestLogger apiRequestLogger
            )
        {
            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(response))
                return new ReCaptcha { IsSuccess = false, Timestamp = DateTime.UtcNow };

            var query = $"?secret={secret}&response={response}";

            if (!string.IsNullOrEmpty(remoteIp))
                query += $"&remoteip={remoteIp}";

            var hub = new IntegrationClient(HttpVerb.GET, IntegrationType.Recaptcha, userIdentifier, organizationIdentifier, apiRequestLogger);
            var validationResponse = hub.RequestString(query);

            return JsonConvert.DeserializeObject<ReCaptcha>(validationResponse);
        }
    }
}
