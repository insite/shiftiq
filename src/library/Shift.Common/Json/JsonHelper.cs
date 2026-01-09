using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shift.Common
{
    public static class JsonHelper
    {
        private const string objTypeNameProperty = "$type-name";
        private const string objTypeNamePropertyOld = "typeName";

        public static string JsonExport<T>(T item, JsonSerializerSettings settings = null)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (settings == null)
                settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

            var jObj = JObject.FromObject(item);

            jObj.Add(objTypeNameProperty, item.GetType().Name);

            return JsonConvert.SerializeObject(jObj, Formatting.Indented, settings);
        }

        public static T JsonImport<T>(string json)
        {
            if (json.IsEmpty())
                throw new ArgumentNullException(nameof(json));

            var jObj = JObject.Parse(json);

            return IsSameType<T>(jObj) ? jObj.ToObject<T>() : throw new ApplicationError("Unexpected JSON object type");
        }

        public static bool IsSameType<T>(string json)
        {
            if (json.IsEmpty())
                throw new ArgumentNullException(nameof(json));

            var jObj = JObject.Parse(json);

            return IsSameType<T>(jObj);
        }

        private static bool IsSameType<T>(JObject jObj)
        {
            var expectedType = typeof(T).Name;
            var readType = string.Empty;

            if (jObj.ContainsKey(objTypeNameProperty))
            {
                readType = jObj[objTypeNameProperty].Value<string>();
                jObj.Remove(objTypeNameProperty);
            }
            else if (jObj.ContainsKey(objTypeNamePropertyOld))
            {
                readType = jObj[objTypeNamePropertyOld].Value<string>();
                jObj.Remove(objTypeNamePropertyOld);
            }

            return readType == expectedType;
        }

        public static string Prettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

        private static readonly JsonSerializerSettings _jsObjSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };

        public static string SerializeJsObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value, _jsObjSettings);
        }

        public static T DeserializeJsObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _jsObjSettings);
        }

        public static string SerializeJson<T>(IEnumerable<T> items, string includes)
        {
            var result = new JArray();

            var properties = Reflector.GetSerializeProperties<T>(includes);

            foreach (var item in items)
            {
                var jObj = new JObject();

                foreach (var p in properties)
                {
                    var value = p.GetValue(item);

                    jObj.Add(p.Name, value == null ? null : JToken.FromObject(value));
                }

                result.Add(jObj);
            }

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}
