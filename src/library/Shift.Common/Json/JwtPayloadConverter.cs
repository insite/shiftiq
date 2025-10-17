using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shift.Common
{
    public class JwtPayloadConverter : JsonConverter<Dictionary<ClaimName, List<string>>>
    {
        private static readonly string[] NumericDateKeys = { "exp", "iat", "nbf", "ttl" };

        private static readonly Dictionary<string, ClaimName> StringToEnum;
        private static readonly Dictionary<ClaimName, string> EnumToString;

        static JwtPayloadConverter()
        {
            StringToEnum = new Dictionary<string, ClaimName>(StringComparer.OrdinalIgnoreCase);
            EnumToString = new Dictionary<ClaimName, string>();

            foreach (ClaimName enumValue in Enum.GetValues(typeof(ClaimName)))
            {
                var member = typeof(ClaimName).GetMember(enumValue.ToString()).FirstOrDefault();
                var attr = member?.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;
                var stringValue = attr?.Value ?? enumValue.ToString();

                StringToEnum[stringValue] = enumValue;
                EnumToString[enumValue] = stringValue;
            }
        }

        public override void WriteJson(JsonWriter writer, Dictionary<ClaimName, List<string>> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                var jsonKey = EnumToString.ContainsKey(kvp.Key) ? EnumToString[kvp.Key] : kvp.Key.ToString();
                writer.WritePropertyName(jsonKey);

                var values = kvp.Value;

                if (values.Count == 1)
                {
                    var singleValue = values[0];
                    if (IsNumericDateKey(jsonKey) && long.TryParse(singleValue, out long numericValue))
                    {
                        writer.WriteValue(numericValue);
                    }
                    else
                    {
                        writer.WriteValue(singleValue);
                    }
                }
                else
                {
                    writer.WriteStartArray();
                    foreach (var item in values)
                    {
                        writer.WriteValue(item);
                    }
                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }

        public override Dictionary<ClaimName, List<string>> ReadJson(JsonReader reader, Type objectType, Dictionary<ClaimName, List<string>> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = new Dictionary<ClaimName, List<string>>();
            var jObject = JObject.Load(reader);

            foreach (var property in jObject.Properties())
            {
                if (!StringToEnum.TryGetValue(property.Name, out var enumKey))
                    continue;

                var token = property.Value;
                List<string> values;

                if (token.Type == JTokenType.Array)
                {
                    values = token.Values().Select(t => t.ToString()).ToList();
                }
                else if (token.Type == JTokenType.String || token.Type == JTokenType.Integer || token.Type == JTokenType.Float || token.Type == JTokenType.Boolean)
                {
                    values = new List<string> { token.ToString() };
                }
                else
                {
                    throw new JsonSerializationException($"Unsupported token type: {token.Type} for key '{property.Name}'");
                }

                result[enumKey] = values;
            }

            return result;
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;

        private static bool IsNumericDateKey(string key)
        {
            return NumericDateKeys.Any(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
        }
    }
}