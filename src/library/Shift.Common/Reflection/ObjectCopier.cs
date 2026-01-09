using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Newtonsoft.Json;

namespace Shift.Common
{
    public static class ObjectCopier
    {
        private class ShallowCopierKey : MultiKey<Type, Type, BindingFlags>
        {
            public ShallowCopierKey(Type source, Type dest, BindingFlags bindingAttr)
                : base(source, dest, bindingAttr)
            {

            }
        }

        private static readonly ConcurrentDictionary<Type, MulticastDelegate> _shallowCopiersByType = new ConcurrentDictionary<Type, MulticastDelegate>();
        private static readonly ConcurrentDictionary<ShallowCopierKey, MulticastDelegate> _shallowCopiersByKey = new ConcurrentDictionary<ShallowCopierKey, MulticastDelegate>();

        public static void ShallowCopyTo<TSource, TDestination>(this TSource source, TDestination destination, BindingFlags bindingAttr = BindingFlags.Default)
            where TSource : class
            where TDestination : class
        {
            GetShallowCopier<TSource, TDestination>(bindingAttr)?.Invoke(source, destination);
        }

        public static Action<TSource, TDestination> GetShallowCopier<TSource, TDestination>(BindingFlags bindingAttr = BindingFlags.Default)
            where TSource : class
            where TDestination : class
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            if (sourceType == destinationType && bindingAttr == BindingFlags.Default)
            {
                return (Action<TSource, TDestination>)_shallowCopiersByType.GetOrAdd(sourceType, k => BuildShallowCopier<TSource, TDestination>(k, k, BindingFlags.Default, false, null, null));
            }
            else
            {
                var copierKey = new ShallowCopierKey(sourceType, destinationType, bindingAttr);

                return (Action<TSource, TDestination>)_shallowCopiersByKey.GetOrAdd(copierKey, k => BuildShallowCopier<TSource, TDestination>(k.Key1, k.Key2, k.Key3, false, null, null));
            }
        }

        public static Action<TSource, TDestination> BuildShallowCopier<TSource, TDestination>(bool exactMatch, ICollection<string> exclude = null, IDictionary<string, string> mapping = null, BindingFlags bindingAttr = BindingFlags.Default)
        {
            return BuildShallowCopier<TSource, TDestination>(typeof(TSource), typeof(TDestination), bindingAttr, exactMatch, exclude, mapping);
        }

        private static Action<TSource, TDestination> BuildShallowCopier<TSource, TDestination>(Type source, Type destination, BindingFlags bindingAttr, bool exactMatch, ICollection<string> exclude, IDictionary<string, string> mapping)
        {
            var matchProperties = new List<(MemberInfo SourceMember, MemberInfo DestinationMember)>();
            var sourceProperties = source.GetProperties(bindingAttr | BindingFlags.Instance | BindingFlags.Public);
            var isSameType = source == destination;

            for (var i = 0; i < sourceProperties.Length; i++)
            {
                var sourceProperty = sourceProperties[i];
                if (exclude.IsNotEmpty() && exclude.Contains(sourceProperty.Name))
                    continue;

                if (!sourceProperty.CanRead || !AllowCopyType(sourceProperty.PropertyType))
                    continue;

                var destinationName = sourceProperty.Name;
                if (mapping.IsNotEmpty() && mapping.ContainsKey(destinationName))
                    destinationName = mapping[destinationName];

                var destinationProperty = destination.GetProperty(destinationName, bindingAttr | BindingFlags.Instance | BindingFlags.Public);
                if (destinationProperty == null)
                {
                    if (exactMatch)
                        throw ApplicationError.Create("Destination property not found: " + destinationName);

                    continue;
                }

                if (!destinationProperty.CanWrite)
                {
                    if (exactMatch)
                        throw ApplicationError.Create("Destination property is read-only: " + destinationName);

                    continue;
                }

                if (!AllowCopyType(destinationProperty.PropertyType) || !destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    if (exactMatch)
                        throw ApplicationError.Create("Destination property type is invalid: " + destinationName);

                    continue;
                }

                (MemberInfo, MemberInfo) item = (sourceProperty, destinationProperty);

                if (isSameType)
                {
                    var sourceField = GetBackingField(sourceProperty);
                    var destinationField = GetBackingField(destinationProperty);

                    if (sourceField != null && destinationField != null)
                        item = (sourceField, destinationField);
                }

                matchProperties.Add(item);
            }

            if (matchProperties.Count == 0)
                return null;

            // TSource src
            var paramSrc = Expression.Parameter(source, "src");

            // TDestination dest
            var paramDest = Expression.Parameter(destination, "dest");

            // dest.{Property1} = src.{Property1};
            // ...
            // dest.{PropertyN} = src.{PropertyN};
            var block = Expression.Block(matchProperties.Select(p =>
            {
                // src.{Property}
                var accessSrcProp = Expression.MakeMemberAccess(paramSrc, p.SourceMember);

                // dest.{Property}
                var accessDestProp = Expression.MakeMemberAccess(paramDest, p.DestinationMember);

                // dest.{Property} = src.{Property}
                return Expression.Assign(accessDestProp, accessSrcProp);
            }));

            // (TSource src, TDestination dest) =>
            // {
            //     dest.{Property1} = src.{Property1};
            //     ...
            //     dest.{PropertyN} = src.{PropertyN};
            // }
            var lambda = Expression.Lambda<Action<TSource, TDestination>>(block, paramSrc, paramDest);

            return lambda.Compile();

            bool AllowCopyType(Type propType)
            {
                return propType.IsPrimitive
                    || propType.IsValueType
                    || propType == typeof(string)
                    || propType.IsGenericType
                        && propType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && propType.GetGenericArguments().All(t => t.IsValueType && t.IsPrimitive);
            }

            FieldInfo GetBackingField(PropertyInfo property)
            {
                return property.DeclaringType.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

        /// <summary>
        /// Return a deep copy of the object using binary serialization.
        /// </summary>
        public static T CloneBinary<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", nameof(source));

            // Don't serialize a null object.
            if (ReferenceEquals(source, null))
                return default;

            var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Perform a deep copy of the object using JSON serialization. 
        /// </summary>
        /// <remarks>
        /// Private members are NOT cloned using this method.
        /// </remarks>
        public static T CloneJson<T>(this T source)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };
            var json = JsonConvert.SerializeObject(source, settings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
