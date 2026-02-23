using System;

using Newtonsoft.Json;

namespace Shift.Common
{
    public class FlagsEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            if (string.IsNullOrEmpty(value))
                return default;

            var flags = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var result = 0;

            foreach (var flag in flags)
            {
                if (Enum.TryParse<T>(flag.Trim(), ignoreCase: true, out var parsed))
                    result |= Convert.ToInt32(parsed);
            }

            return (T)(object)result;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var formatted = value.ToString().ToLowerInvariant().Replace(" ", "");
            writer.WriteValue(formatted);
        }
    }
}
