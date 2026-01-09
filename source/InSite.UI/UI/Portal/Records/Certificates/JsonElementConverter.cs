using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace InSite.UI.Portal.Records.Certificates
{
    public class JsonElementConverter : JsonConverter
    {
        #region Classes

        private class InternalContractResolver : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type type)
            {
                return typeof(CertificateBaseElement).IsAssignableFrom(type) && !type.IsAbstract
                    ? null
                    : base.ResolveContractConverter(type);
            }
        }

        private class InternalJsonSerializer : JsonSerializer
        {
            public InternalJsonSerializer()
                : base()
            {
                ContractResolver = new InternalContractResolver();
            }
        }

        #endregion

        public override bool CanConvert(Type type) => type.IsSubclassOf(typeof(CertificateBaseElement));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);
            var type = jObj["type"].Value<string>();
            var inSerializer = new InternalJsonSerializer();

            if (type == "text")
                return jObj.ToObject<CertificateTextElement>(inSerializer);

            if (type == "image")
                return jObj.ToObject<CertificateImageElement>(inSerializer);

            throw new Exception("Unexpected certificate element type: " + type);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string type;

            if (value is CertificateTextElement)
                type = "text";
            else if (value is CertificateImageElement)
                type = "image";
            else
                throw new Exception("Unexpected certificate element type: " + value.GetType());

            var inSerializer = new InternalJsonSerializer();
            var jObj = JObject.FromObject(value, inSerializer);
            jObj.AddFirst(new JProperty("type", type));
            jObj.WriteTo(writer);
        }
    }
}