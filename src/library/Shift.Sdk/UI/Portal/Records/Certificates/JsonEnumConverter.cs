using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    public class JsonEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type.IsEnum;

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new Exception("Unexpected token type: " + reader.TokenType);

            var str = (string)reader.Value;
            var result = Enum.Parse(type, str);
            if (!Enum.IsDefined(type, result))
                throw new Exception("Unexpected enum value: " + str ?? "(null)");

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            writer.WriteValue(name);
        }
    }
}