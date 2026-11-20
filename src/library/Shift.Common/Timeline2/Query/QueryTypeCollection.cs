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

                // The result type is the argument of the Query<TResult> base class, not of an interface. IQuery
                // is not generic, so asking it for its first generic argument threw an index out of range for
                // every query type, which made this collection unusable for any assembly that declares one.
                var resultType = GetResultTypeFromBase(query);

                if (resultType == null)
                    throw new InvalidOperationException($"{key} does not derive from Query<TResult>.");

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

                resource.Routes.Add(queries[key].FullName);

                list.Add(resource);
            }

            return list;
        }

        private static Type GetResultTypeFromBase(Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Query<>))
                    return type.GetGenericArguments()[0];

                type = type.BaseType;
            }

            return null;
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