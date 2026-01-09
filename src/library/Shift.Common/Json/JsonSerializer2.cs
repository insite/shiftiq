using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Shift.Common
{
    public class JsonSerializer2 : IJsonSerializerBase
    {
        static readonly Dictionary<string, JsonResolver2> Resolvers = new Dictionary<string, JsonResolver2>();

        public string Serialize<T>(T value)
        {
            return Serialize(value, JsonPurpose.Storage, false, null);
        }

        public T Deserialize<T>(string value)
        {
            return Deserialize<T>(typeof(T), value, JsonPurpose.Storage, false, null);
        }

        public string Serialize(object value, JsonPurpose mode, bool disablePropertyConverters = false, string[] excludeProperties = null)
        {
            var settings = CreateSerializerSettings(mode, disablePropertyConverters, excludeProperties);

            return JsonConvert.SerializeObject(value, settings);
        }

        public T Deserialize<T>(Type type, string value, JsonPurpose mode, bool disablePropertyConverters = false, string[] excludeProperties = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;

            var settings = CreateSerializerSettings(mode, disablePropertyConverters, excludeProperties);

            return (T)JsonConvert.DeserializeObject(value, type, settings);
        }

        private JsonSerializerSettings CreateSerializerSettings(JsonPurpose mode, bool disablePropertyConverters, string[] excludeProperties)
        {
            var settings = new JsonSerializerSettings();

            if (mode == JsonPurpose.Storage)
            {
                settings.ContractResolver = GetResolver(disablePropertyConverters, excludeProperties);
                settings.DefaultValueHandling = DefaultValueHandling.Include;
                settings.Formatting = Formatting.None;
                settings.NullValueHandling = NullValueHandling.Include;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.TypeNameHandling = TypeNameHandling.None;
            }
            else if (mode == JsonPurpose.Display)
            {
                settings.ContractResolver = GetResolver(disablePropertyConverters, excludeProperties);
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.Formatting = Formatting.Indented;
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.TypeNameHandling = TypeNameHandling.None;
            }

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        private JsonResolver2 GetResolver(bool disablePropertyConverters, string[] excludeProperties)
        {
            var resolverKey = $"Property converters " + (disablePropertyConverters ? "disabled" : "enabled");

            if (excludeProperties == null || excludeProperties.Length == 0)
                resolverKey = ". Including All Properties.";
            else
                resolverKey = ". Excluding properties: " + string.Join(", ", excludeProperties) + ".";

            lock (this)
            {
                if (!Resolvers.ContainsKey(resolverKey))
                    Resolvers.Add(resolverKey, new JsonResolver2(disablePropertyConverters, excludeProperties));
            }

            return Resolvers[resolverKey];
        }
    }
}