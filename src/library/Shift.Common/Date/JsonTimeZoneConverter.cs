using System;

using Newtonsoft.Json;

namespace Shift.Common
{
    public class JsonTimeZoneConverter : JsonConverter<TimeZoneInfo>
    {
        public override TimeZoneInfo ReadJson(JsonReader reader, Type type, TimeZoneInfo value, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                if (reader.TokenType == JsonToken.StartObject)
                    return serializer.Deserialize<TimeZoneInfo>(reader);

                throw new ApplicationError("Unexpected token type: " + reader.TokenType);
            }

            var id = (string)reader.Value;

            if (string.IsNullOrEmpty(id))
                return null;

            var result = TimeZones.GetInfo(id);

            if (result == null)
                throw new ApplicationError("Unsupported time zone: " + id ?? "(null)");

            return result;
        }

        public override void WriteJson(JsonWriter writer, TimeZoneInfo value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue(value.Id);
            else
                writer.WriteUndefined();
        }
    }
}
