using System;

using Shift.Common;

namespace Shift.Common
{
    public class QueryBuilder
    {
        private readonly QueryTypeCollection _queryTypes;
        private readonly IJsonSerializerBase _serializer;

        public QueryBuilder(QueryTypeCollection queryTypes, IJsonSerializerBase serializer)
        {
            _queryTypes = queryTypes;
            _serializer = serializer;
        }

        public QueryBuilder() { }

        public Type GetQueryType(string queryName)
        {
            var queryType = _queryTypes.GetQueryType(queryName);

            if (queryType == null)
                throw new BadQueryException($"{queryName} is not a registered query type.");

            return queryType;
        }

        public Type GetResultType(Type queryType)
        {
            var resultType = _queryTypes.GetResultType(queryType);

            if (resultType == null)
                throw new BadQueryException($"{queryType.Name} has no registered result type.");

            return resultType;
        }

        /// <remarks>
        /// Contructs a query object that derives from the Query type, which gurantees a strongly-
        /// typed result object. Criteria is implemented by the properties of the query; these 
        /// property values are deserialized from the HTTP request body. 
        /// </remarks>
        public object BuildQuery(Type queryType, Type resultType, string requestBody, QueryFilter filter)
        {
            var queryObject = CreateQuery(queryType, resultType, requestBody, filter);

            if (queryObject == null)
                throw new BadQueryException($"{queryType.Name} query object creation failed unexpectedly.");

            return queryObject;
        }

        private object CreateQuery(Type queryType, Type resultType, string requestBody, QueryFilter filter)
        {
            var deserializer = new QuerySerializer(_serializer);

            var deserializeMethod = typeof(QuerySerializer).GetMethod(nameof(QuerySerializer.Deserialize), new[] { typeof(Type), typeof(string) });

            var genericDeserializeMethod = deserializeMethod.MakeGenericMethod(resultType);

            var queryObject = genericDeserializeMethod.Invoke(deserializer, new object[] { queryType, requestBody });

            if (queryObject == null)
                queryObject = Activator.CreateInstance(queryType);

            var filterProperty = queryType.GetProperty("Filter");

            if (filterProperty != null)
            {
                // If the Filter property value is null in the deserialized query, then the HTTP request
                // body does specify any filter. In this case, look for filter property values in the
                // query string as a fail-safe. This ensures a query always has a non-null filter, which
                // ensures queries are always paged by default.

                if (filterProperty.GetValue(queryObject) == null)
                    filterProperty.SetValue(queryObject, filter);
            }

            var originProperty = queryType.GetProperty("Origin");

            if (originProperty != null)
            {
                if (originProperty.GetValue(queryObject) == null)
                    originProperty.SetValue(queryObject, new Origin { When = DateTimeOffset.Now });
            }

            return queryObject;
        }
    }
}
