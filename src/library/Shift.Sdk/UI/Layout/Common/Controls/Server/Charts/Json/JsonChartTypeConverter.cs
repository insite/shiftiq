using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    public class JsonChartTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type == typeof(ChartType);

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumValue = (ChartType)value;
            if (enumValue == ChartType.Pie)
                writer.WriteValue("pie");
            else if (enumValue == ChartType.Bar)
                writer.WriteValue("bar");
            else if (enumValue == ChartType.Line)
                writer.WriteValue("line");
            else
                throw new NotImplementedException($"Unexpected chart type: {value}");
        }
    }
}
