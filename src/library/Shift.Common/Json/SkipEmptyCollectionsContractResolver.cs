using System.Collections;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shift.Common
{
    public class SkipEmptyCollectionsContractResolver : DefaultContractResolver
    {
        public SkipEmptyCollectionsContractResolver()
        {
            NamingStrategy = new DefaultNamingStrategy();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
            {
                property.ShouldSerialize = instance =>
                {
                    var value = property.ValueProvider?.GetValue(instance) as IEnumerable;
                    return value?.GetEnumerator().MoveNext() == true;
                };
            }

            return property;
        }
    }
}
