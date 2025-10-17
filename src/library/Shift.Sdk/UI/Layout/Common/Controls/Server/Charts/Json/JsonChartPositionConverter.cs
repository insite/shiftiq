using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    public class JsonChartPositionConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type == typeof(ChartPosition);

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumValue = (ChartPosition)value;
            if (enumValue == ChartPosition.Top)
                writer.WriteValue("top");
            else if (enumValue == ChartPosition.Right)
                writer.WriteValue("right");
            else if (enumValue == ChartPosition.Left)
                writer.WriteValue("left");
            else if (enumValue == ChartPosition.Bottom)
                writer.WriteValue("bottom");
            else
                throw new NotImplementedException($"Unexpected chart type: {value}");
        }
    }
}
