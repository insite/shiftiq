using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;

namespace Shift.Common.Json
{
    internal sealed class Resolver : DefaultContractResolver
    {
        private readonly string[] _exclusions;
        private readonly bool _ignoreAttributes;

        public Resolver(string[] exclusions, bool ignoreAttributes)
        {
            _exclusions = exclusions;
            _ignoreAttributes = ignoreAttributes;
        }

        /// <summary>
        /// Exclude properties that we don't want in the serialized JSON output, and sort properties
        /// alphabetically.
        /// </summary>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (_exclusions != null)
            {
                properties = properties
                    .Where(p => !_exclusions.Contains(p.PropertyName))
                    .OrderBy(p => p.PropertyName)
                    .ToList();
            }

            if (_ignoreAttributes)
            {
                foreach (var prop in properties)
                {
                    prop.Converter = null;
                    prop.Ignored = false;
                }
            }

            return properties;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            var result = base.ResolveContractConverter(objectType);

            return result;
        }
    }

    internal sealed class CommandContractResolver : DefaultContractResolver
    {
        private readonly Type _commandType;
        private readonly ConcurrentDictionary<Type, bool> _multilingualDictionaryDerivedTypes = new ConcurrentDictionary<Type, bool>();
        private readonly ConcurrentDictionary<Type, bool> _commandDerivedTypes = new ConcurrentDictionary<Type, bool>();
        private readonly IReadOnlyCollection<string> _excludeProperties;

        public CommandContractResolver(Type commandType, string[] excludeProperties)
        {
            _commandType = commandType;
            _excludeProperties = new HashSet<string>(excludeProperties);
        }

        /// <summary>
        /// Exclude properties that we don't want in the serialized JSON output, and sort properties
        /// alphabetically.
        /// </summary>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (_commandDerivedTypes.GetOrAdd(type, t => _commandType.IsAssignableFrom(t)))
            {
                properties = properties
                    .Where(p => !_excludeProperties.Contains(p.PropertyName))
                    .OrderBy(p => p.PropertyName)
                    .ToList();
            }

            return properties;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(MultilingualString))
                return new MultilingualString.MultilingualStringJsonConverter(true);
            else if (objectType == typeof(ContentContainerItem))
                return new ContentContainerItem.ContentContainerItemJsonConverter(true);
            else if (objectType == typeof(ContentContainer))
                return new ContentContainer.ContentContainerJsonConverter(true);
            else if (_multilingualDictionaryDerivedTypes.GetOrAdd(objectType, t => typeof(MultilingualDictionary).IsAssignableFrom(t)))
                return new MultilingualDictionary.MultilingualDictionaryJsonConverter(true);

            var result = base.ResolveContractConverter(objectType);

            return result;
        }
    }
}
