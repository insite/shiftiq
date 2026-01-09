using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonConverter(typeof(HotspotShapeConverter))]
    public abstract class HotspotShape
    {
        public abstract HotspotShape Clone();

        public abstract void Resize(HotspotImage.ResizeEventArgs args);

        public abstract void Read(string value);

        public abstract bool IsPointInside(int x, int y);

        public abstract bool IsEqual(HotspotShape other);

        public abstract void CopyTo(HotspotShape other);

        public static HotspotShape FromString(string value)
        {
            var idIndex = value.IndexOf(' ');
            if (idIndex == -1)
                throw ApplicationError.Create("Wrong string format: " + value);

            var shapeId = value.Substring(0, idIndex);

            if (shapeId == HotspotShapeCircle.ShapeID)
                return new HotspotShapeCircle(value);
            else if (shapeId == HotspotShapeRectangle.ShapeID)
                return new HotspotShapeRectangle(value);

            throw ApplicationError.Create("Unexpected type name: " + value);
        }

        private class HotspotShapeConverter : JsonConverter<HotspotShape>
        {
            public override HotspotShape ReadJson(JsonReader reader, Type type, HotspotShape value, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;

                if (reader.TokenType != JsonToken.String)
                    throw new Exception("Wrong token type");

                return HotspotShape.FromString((string)reader.Value);
            }

            public override void WriteJson(JsonWriter writer, HotspotShape value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
