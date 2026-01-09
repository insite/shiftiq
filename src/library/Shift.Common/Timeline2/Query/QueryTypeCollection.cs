using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Shift.Common;

namespace Shift.Common
{
    public class QueryTypeCollection
    {
        private readonly Reflector _reflector;
        private readonly Dictionary<string, Type> _queries;
        private readonly Dictionary<Type, Type> _results;
        private readonly List<Resource> _resources;

        public QueryTypeCollection(Assembly assembly)
        {
            _reflector = new Reflector();

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            _queries = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfRawGeneric(typeof(Query<>), t))
                .ToDictionary(t => t.Name.ToLower(), t => t);

            _results = new Dictionary<Type, Type>();

            foreach (var key in _queries.Keys)
            {
                var query = _queries[key];

                var iQueryInterface = query.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

                if (iQueryInterface == null)
                    throw new Exception($"{key} does not implement the IQuery interface.");

                var resultType = iQueryInterface.GetGenericArguments()[0];

                _results.Add(query, resultType);
            }

            _resources = CreateResources(_queries);
        }

        private List<Resource> CreateResources(Dictionary<string, Type> queries)
        {
            var list = new List<Resource>();

            foreach (var key in queries.Keys)
            {
                var name = _reflector.GetResourceName(queries[key]);

                var resource = new Resource(name);

                resource.Aliases.Add(queries[key].FullName);

                list.Add(resource);
            }

            return list;
        }

        private bool IsSubclassOfRawGeneric(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == current)
                    return true;

                type = type.BaseType;
            }
            return false;
        }

        public Type GetQueryType(string name)
        {
            var typeNameVariations = GetTypeNameVariations(name);

            foreach (var typeName in typeNameVariations)
            {
                if (_queries.TryGetValue(typeName, out Type type))
                {
                    return type;
                }
            }

            return null;
        }

        private List<string> GetTypeNameVariations(string typeName)
        {
            var list = new List<string>();

            if (string.IsNullOrWhiteSpace(typeName))
                return list;

            list.Add(RemoveHyphens(typeName));

            const string prefix = "Query";

            if (!typeName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                list.Add($"Query{RemoveHyphens(typeName)}");
            }

            for (var i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Trim().ToLower();
            }

            return list;
        }

        public static string RemoveHyphens(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("-", string.Empty);
        }

        public Type GetResultType(Type query)
        {
            if (_results.TryGetValue(query, out Type type))
            {
                return type;
            }

            return null;
        }

        public List<Resource> GetResources()
            => _resources;
    }
}