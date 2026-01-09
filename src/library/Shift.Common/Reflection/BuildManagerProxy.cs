using System;
using System.Collections.Concurrent;

namespace Shift.Common
{
    /// <summary>
    /// Mimic the implementation of BuildManager.GetType without requiring a dependency on System.Web
    /// </summary>
    public static class BuildManagerProxy
    {
        private static readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

        public static Type GetType(string typeName)
        {
            return GetType(typeName, throwOnError: true, ignoreCase: false);
        }

        public static Type GetType(string typeName, bool throwOnError)
        {
            return GetType(typeName, throwOnError, ignoreCase: false);
        }

        public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            // Use cache to boost performance (BuildManager caches types)

            var cacheKey = $"{typeName}:{ignoreCase}";
            if (_typeCache.TryGetValue(cacheKey, out Type cachedType))
                return cachedType;

            Type type = null;

            // Step 1: If the type name contains an assembly qualifier, then try Type.GetType first

            if (typeName.Contains(","))
            {
                type = Type.GetType(typeName, throwOnError: false, ignoreCase);
                if (type != null)
                {
                    ValidateAndCacheType(type, typeName, cacheKey);
                    return type;
                }
            }

            // Step 2: Try standard Type.GetType for types in mscorlib and in the current assembly

            type = Type.GetType(typeName, throwOnError: false, ignoreCase);
            if (type != null)
            {
                ValidateAndCacheType(type, typeName, cacheKey);
                return type;
            }

            // Step 3: Search key framework assemblies

            var frameworkAssemblies = new[]
            {
                typeof(Exception).Assembly,                  // mscorlib/System.Runtime
                typeof(System.Data.DataException).Assembly,  // System.Data
                typeof(System.IO.IOException).Assembly,      // System.IO
            };

            foreach (var assembly in frameworkAssemblies)
            {
                if (assembly != null)
                {
                    try
                    {
                        type = assembly.GetType(typeName, throwOnError: false, ignoreCase);
                        if (type != null)
                        {
                            ValidateAndCacheType(type, typeName, cacheKey);
                            return type;
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Malformed type name, continue searching
                    }
                }
            }

            // Step 4: Search all loaded assemblies

            type = SearchLoadedAssemblies(typeName, ignoreCase);
            if (type != null)
            {
                ValidateAndCacheType(type, typeName, cacheKey);
                return type;
            }

            // Step 5: If the type was not found then throw an exception

            if (throwOnError)
            {
                throw new TypeLoadException($"Could not load type '{typeName}' from any loaded assembly.");
            }

            return null;
        }

        private static Type SearchLoadedAssemblies(string typeName, bool ignoreCase)
        {
            // Get all assemblies currently loaded in the AppDomain

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Iterate in reverse order to check the most recently loaded assemblies first

            for (int i = assemblies.Length - 1; i >= 0; i--)
            {
                try
                {
                    var type = assemblies[i].GetType(typeName, throwOnError: false, ignoreCase);
                    if (type != null)
                        return type;
                }
                catch (ArgumentException)
                {
                    // Malformed type name for this assembly, continue
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Assembly dependency not found, skip
                }
                catch (System.IO.FileLoadException)
                {
                    // Assembly could not be loaded, skip
                }
            }

            return null;
        }

        private static void ValidateAndCacheType(Type type, string typeName, string cacheKey)
        {
            // Validate that the type derives from Exception

            if (!typeof(Exception).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"The type '{typeName}' must be assignable from System.Exception. " +
                    $"The resolved type '{type.FullName}' does not inherit from Exception.");
            }

            // Cache the validated type

            _typeCache.TryAdd(cacheKey, type);
        }
    }
}
