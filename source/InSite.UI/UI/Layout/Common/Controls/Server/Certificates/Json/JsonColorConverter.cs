using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI.Certificates.Json
{
    public class JsonColorConverter : JsonConverter
    {
        public override bool CanConvert(Type type) => type == typeof(System.Drawing.Color);

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new ApplicationError("Unexpected token type: " + reader.TokenType);

            var str = (string)reader.Value;

            return str[0] == '#' 
                ? System.Drawing.ColorTranslator.FromHtml(str) 
                : System.Drawing.Color.FromName(str);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (System.Drawing.Color)value;

            if (color.IsNamedColor)
                writer.WriteValue(color.Name);
            else
                writer.WriteValue("#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"));
        }
    }
}