using System;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    public class JsonChartInteractionModeConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type == typeof(ChartInteractionMode);

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var mode = (ChartInteractionMode)value;
            if (mode == ChartInteractionMode.Point)
                writer.WriteValue("point");
            else if (mode == ChartInteractionMode.Nearest)
                writer.WriteValue("nearest");
            else if (mode == ChartInteractionMode.Index)
                writer.WriteValue("index");
            else if (mode == ChartInteractionMode.Dataset)
                writer.WriteValue("dataset");
            else if (mode == ChartInteractionMode.X)
                writer.WriteValue("x");
            else if (mode == ChartInteractionMode.Y)
                writer.WriteValue("y");
            else
                throw new NotImplementedException($"Unexpected chart type: {value}");
        }
    }
}
