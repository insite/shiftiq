using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    public class JsonCaseInsensitiveDictionaryConverter<TValue> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<string, TValue>).IsAssignableFrom(objectType)
                || objectType == typeof(Dictionary<string, TValue>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dictionary = new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);

            serializer.Populate(reader, dictionary);

            return dictionary;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
