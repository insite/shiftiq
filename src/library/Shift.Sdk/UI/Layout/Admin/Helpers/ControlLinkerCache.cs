using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using Shift.Common;

namespace Shift.Sdk.UI
{
    /// <summary>
    /// This class manages a dictionary cache that maps each class type to a function. The function finds the get 
    /// accessor for a property that has a specific name and type. It throws an exception if the get accessor returns a 
    /// value that cannot be assigned to the desired type.
    /// </summary>
    public static class ControlLinkerCache
    {
        public abstract class Accessor
        {
            public Accessor(Type _)
            {

            }

            protected static Func<object, object> Find(Type t, string name)
            {
                var tMember = t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? (MemberInfo)t.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

                return tMember == null ? Null : tMember.BuildGetter();
            }

            protected static Func<object, TValue> Find<TValue>(Type t, string name)
            {
                var tMember = t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? (MemberInfo)t.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

                if (tMember == null)
                    return Default<TValue>();

                var tValue = typeof(TValue);
                if (!tValue.IsAssignableFrom(tMember.GetUnderlyingType()))
                    throw new TypeMismatchException($"The control {t.FullName}.{name} must be assignable from an object of this type: {tValue.FullName}. Instead it has this unexpected type: {tMember.GetUnderlyingType().FullName}.");

                return tMember.BuildGetterWithSpecifiedValue<TValue>();
            }

            protected static object Null(object o) 
                => null;

            protected static Func<object, TValue> Default<TValue>() 
                => x => default(TValue);
        }

        private static ReadOnlyDictionary<Type, Accessor> _cache;

        public static Accessor Get(object container)
        {
            return _cache.TryGetValue(container.GetType().BaseType, out var data) ? data : null;
        }

        public static void Init(params Assembly[] assemblies)
        {
            var baseItemType = typeof(Accessor);
            var allTypes = assemblies.SelectMany(x => x.GetTypes());
            var cache = new Dictionary<Type, Accessor>();

            foreach (var itemType in allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(baseItemType)))
            {
                var declaringType = itemType.DeclaringType;
                if (declaringType == null)
                    continue;

                while (declaringType.DeclaringType != null)
                    declaringType = declaringType.DeclaringType;

                if (declaringType.IsGenericType && declaringType.IsGenericTypeDefinition)
                {
                    foreach (var t in allTypes)
                    {
                        if (!t.IsClass || t.IsAbstract)
                            continue;

                        var containerGeneric = t.GetAllAscendants()
                            .FirstOrDefault(d => d.IsGenericType && !d.IsGenericTypeDefinition && d.GetGenericTypeDefinition() == declaringType);

                        if (containerGeneric == null)
                            continue;

                        var genericItemType = itemType.MakeGenericType(containerGeneric.GenericTypeArguments);
                        var itemCtor = genericItemType.GetConstructor(new[] { typeof(Type) });
                        if (itemCtor == null)
                            throw ApplicationError.Create("Constructor not found: " + itemType.FullName);

                        cache.Add(t, (Accessor)itemCtor.Invoke(new[] { t }));
                    }
                }
                else
                {
                    var itemCtor = itemType.GetConstructor(new[] { typeof(Type) });
                    if (itemCtor == null)
                        throw ApplicationError.Create("Constructor not found: " + itemType.FullName);

                    foreach (var containerType in allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(declaringType)))
                        cache.Add(containerType, (Accessor)itemCtor.Invoke(new[] { containerType }));
                }
            }

            _cache = new ReadOnlyDictionary<Type, Accessor>(cache);
        }
    }
}