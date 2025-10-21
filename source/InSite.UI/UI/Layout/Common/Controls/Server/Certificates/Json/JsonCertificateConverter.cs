using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Shift.Common;

namespace InSite.Common.Web.UI.Certificates.Json
{
    public class JsonCertificateConverter : JsonConverter
    {
        #region Classes

        private class InternalContractResolver : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type type)
            {
                return typeof(BaseCertificate).IsAssignableFrom(type) && !type.IsAbstract
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

        public override bool CanConvert(Type type) => type.IsSubclassOf(typeof(BaseCertificate));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);
            var type = jObj["type"].Value<string>();
            var inSerializer = new InternalJsonSerializer();

            if (type == "cmds")
                return jObj.ToObject<CmdsTrainingCertificate>(inSerializer);

            if (type == "custom")
                return jObj.ToObject<CustomCertificate>(inSerializer);

            if (type == "iecbc-1")
                return jObj.ToObject<iecbcCertificate1>(inSerializer);

            if (type == "iecbc-2")
                return jObj.ToObject<iecbcCertificate2>(inSerializer);

            throw new ApplicationError("Unexpected certificate element type: " + (type ?? "null"));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string type;

            if (value is CmdsTrainingCertificate)
                type = "cmds";
            else if (value is CustomCertificate)
                type = "custom";
            else if (value is iecbcCertificate1)
                type = "iecbc-1";
            else if (value is iecbcCertificate2)
                type = "iecbc-2";
            else
                throw new ApplicationError("Unexpected certificate type: " + value.GetType());

            var inSerializer = new InternalJsonSerializer();
            var jObj = JObject.FromObject(value, inSerializer);
            jObj.AddFirst(new JProperty("type", type));
            jObj.WriteTo(writer);
        }
    }
}