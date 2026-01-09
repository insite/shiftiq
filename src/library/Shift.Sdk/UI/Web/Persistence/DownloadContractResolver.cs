using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shift.Sdk.UI
{
    public class DownloadContractResolver : DefaultContractResolver
    {
        private Dictionary<Type, HashSet<string>> _types = new Dictionary<Type, HashSet<string>>();

        public void AddProperties(Type t, IEnumerable<string> props)
        {
            if (!_types.ContainsKey(t))
                _types.Add(t, new HashSet<string>());

            var pSet = _types[t];
            foreach (var p in props)
                pSet.Add(p);
        }

        public void RemoveProperties(Type t)
        {
            _types.Remove(t);
        }

        public void RemoveProperties(Type t, IEnumerable<string> props)
        {
            if (!_types.ContainsKey(t))
                return;

            var pSet = _types[t];
            foreach (var p in props)
                pSet.Remove(p);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperties(type, memberSerialization);
            if (!_types.ContainsKey(type))
                return result;

            var pSet = _types[type];

            return result.Where(x => pSet.Contains(x.PropertyName)).ToList();
        }
    }
}