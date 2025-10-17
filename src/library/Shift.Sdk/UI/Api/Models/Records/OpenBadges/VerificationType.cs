using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Shift.Sdk.UI
{
    [JsonConverter(typeof(VerificationTypeJsonConverter))]
    public enum VerificationType
    {
        HostedBadge,
        SignedBadge
    }

    internal class VerificationTypeJsonConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (value.Equals("HostedBadge", StringComparison.OrdinalIgnoreCase) || value.Equals("hosted", StringComparison.OrdinalIgnoreCase))
                    return VerificationType.HostedBadge;

                if (value.Equals("SignedBadge", StringComparison.OrdinalIgnoreCase) || value.Equals("signed", StringComparison.OrdinalIgnoreCase))
                    return VerificationType.SignedBadge;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}