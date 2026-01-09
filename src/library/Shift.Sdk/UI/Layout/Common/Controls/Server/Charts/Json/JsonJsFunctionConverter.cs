using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    public class JsonJsFunctionConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type == typeof(string);

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue((string)value);
        }
    }
}
