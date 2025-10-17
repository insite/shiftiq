using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class JsonColorConverter : JsonConverter
    {
        public override bool CanConvert(Type type) =>
            throw new NotImplementedException();

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IEnumerable<System.Drawing.Color> colors)
            {
                var array = new JArray();
                foreach (var c in colors)
                    array.Add(FormatColor(c));

                array.WriteTo(writer);
            }
            else if (value is System.Drawing.Color color)
            {
                writer.WriteValue(FormatColor(color));
            }
        }

        private static string FormatColor(System.Drawing.Color c) => "#" + StringHelper.ByteToHex(c.R) + StringHelper.ByteToHex(c.G) + StringHelper.ByteToHex(c.B);
    }
}
