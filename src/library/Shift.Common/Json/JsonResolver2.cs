using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shift.Common
{
    internal sealed class JsonResolver2 : DefaultContractResolver
    {
        private readonly bool _disablePropertyConverters;

        private readonly string[] _excludeProperties;

        public JsonResolver2(bool disablePropertyConverters, string[] excludeProperties)
        {
            _disablePropertyConverters = disablePropertyConverters;
            _excludeProperties = excludeProperties;
        }

        /// <summary>
        /// Exclude properties that we don't want in the serialized JSON output, and sort properties alphabetically.
        /// </summary>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (_excludeProperties != null && _excludeProperties.Length > 0)
            {
                properties = properties
                    .Where(p => !_excludeProperties.Contains(p.PropertyName, new CaseInsensitiveStringComparer()))
                    .OrderBy(p => p.PropertyName)
                    .ToList();
            }

            if (_disablePropertyConverters)
            {
                foreach (var property in properties)
                {
                    property.Converter = null;
                    property.Ignored = false;
                }
            }

            return properties;
        }

        private class CaseInsensitiveStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(string obj) => obj.GetHashCode();
        }
    }
}