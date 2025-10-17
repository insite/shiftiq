using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shift.Sdk.UI
{
    public class JsonDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => 
            throw new NotImplementedException();

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                if (value is IEnumerable<DateTime> values)
                {
                    var array = new JArray();
                    foreach (var dt in values)
                        array.Add(FormatDate(dt));

                    array.WriteTo(writer);
                }
                else if (value is DateTime date)
                {
                    writer.WriteValue(FormatDate(date));
                }
            }
            else
                writer.WriteValue("null");
        }

        private static string FormatDate(DateTime dt) => dt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffZ");
    }
}
