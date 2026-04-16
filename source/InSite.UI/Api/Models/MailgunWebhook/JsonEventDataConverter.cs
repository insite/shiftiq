using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace InSite.Api.Models.MailgunWebhook
{
    internal class JsonEventDataConverter : JsonConverter
    {
        #region Classes

        private class InternalContractResolver : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type type)
            {
                return typeof(EventDataBase).IsAssignableFrom(type) && !type.IsAbstract
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

        public override bool CanConvert(Type type) => type.IsSubclassOf(typeof(EventDataBase));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);
            var type = jObj["event"].Value<string>();
            var inSerializer = new InternalJsonSerializer();

            if (type == "accepted")
                return jObj.ToObject<EventDataAccepted>(inSerializer);

            if (type == "delivered")
                return jObj.ToObject<EventDataDelivered>(inSerializer);

            if (type == "failed" || type == "temporary_fail" || type == "permanent_fail")
                return jObj.ToObject<EventDataFailed>(inSerializer);

            if (type == "opened")
                return jObj.ToObject<EventDataOpened>(inSerializer);

            if (type == "clicked")
                return jObj.ToObject<EventDataClicked>(inSerializer);

            if (type == "unsubscribed")
                return jObj.ToObject<EventDataUnsubscribed>(inSerializer);

            if (type == "complained")
                return jObj.ToObject<EventDataComplained>(inSerializer);

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}