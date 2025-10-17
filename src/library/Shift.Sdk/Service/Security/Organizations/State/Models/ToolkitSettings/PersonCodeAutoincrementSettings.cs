using System;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable, JsonConverter(typeof(PersonCodeAutoincrementSettingsJsonConverter))]
    public class PersonCodeAutoincrementSettings
    {
        private const int DefaultStartNumber = 40000;
        private const string DefaultFormat = "{0:0000000}";

        public bool Enabled { get; set; }

        [DefaultValue(DefaultStartNumber)]
        public int StartNumber { get; set; } = DefaultStartNumber;

        [DefaultValue(DefaultFormat)]
        public string Format { get; set; } = DefaultFormat;

        public bool IsShallowEqual(PersonCodeAutoincrementSettings other)
        {
            return Enabled == other.Enabled
                && StartNumber == other.StartNumber
                && Format.NullIfEmpty() == other.Format.NullIfEmpty();
        }

        private class PersonCodeAutoincrementSettingsJsonConverter : JsonConverter
        {
            public override bool CanWrite => false;

            public override bool CanConvert(Type type) => throw new NotImplementedException();

            public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Boolean)
                    return new PersonCodeAutoincrementSettings { Enabled = (bool)reader.Value };

                if (reader.TokenType != JsonToken.StartObject)
                    throw new Exception("Unexpected token type: " + reader.TokenType);

                var jObj = JObject.Load(reader);
                var result = new PersonCodeAutoincrementSettings();

                serializer.Populate(jObj.CreateReader(), result);

                return result;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
        }
    }
}
