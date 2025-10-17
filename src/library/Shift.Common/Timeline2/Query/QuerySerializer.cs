using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Common
{
    /// <summary>
    /// Provides functions to convert between instances of IQuery and SerializedQuery.
    /// </summary>
    public class QuerySerializer
    {
        private readonly IJsonSerializerBase _serializer;

        public QuerySerializer(IJsonSerializerBase serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// Returns a deserialized query.
        /// </summary>
        public IQuery<TResult> Deserialize<TResult>(Type queryType, string queryCriteria)
        {
            try
            {
                var queryObject = _serializer.Deserialize<IQuery<TResult>>(queryType, queryCriteria, JsonPurpose.Storage);

                if (queryObject != null)
                {
                    if (queryObject.Origin != null)
                    {
                        if (queryObject.Origin.When == DateTimeOffset.MinValue)
                        {
                            queryObject.Origin.When = DateTimeOffset.Now;
                        }
                    }
                }

                return queryObject;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"{ex.Message} Query: Type = {queryType.Name}, Criteria = [{queryCriteria}]", ex);
            }
        }

        /// <summary>
        /// Returns a deserialized query.
        /// </summary>
        public IQuery<TResult> Deserialize<TResult>(SerializedQuery query)
        {
            try
            {
                var criteria = _serializer.Deserialize<IQuery<TResult>>(Type.GetType(query.Class), query.Criteria, JsonPurpose.Storage);

                criteria.Identifier = query.Identifier;

                criteria.Origin = query.Origin != null
                    ? _serializer.Deserialize<Origin>(query.Origin)
                    : null;

                criteria.Filter = query.Filter != null
                    ? _serializer.Deserialize<QueryFilter>(query.Filter)
                    : null;

                criteria.Texts = query.Texts != null
                    ? _serializer.Deserialize<Dictionary<Guid, string>>(query.Texts)
                    : null;

                return criteria;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"{ex.Message} Query: Type = {query.Class}, Identifier = {query.Identifier}, Criteria = [{query.Criteria}]", ex);
            }
        }

        /// <summary>
        /// Returns a serialized query.
        /// </summary>
        public SerializedQuery Serialize<TResult>(IQuery<TResult> query)
        {
            var id = query.Identifier ?? Guid.Empty;

            var origin = query.Origin != null
                ? _serializer.Serialize(query.Origin, JsonPurpose.Storage)
                : null;

            var filter = query.Filter != null
                ? _serializer.Serialize(query.Filter, JsonPurpose.Storage)
                : null;

            var texts = query.Texts != null
                ? _serializer.Serialize(query.Texts, JsonPurpose.Storage)
                : null;

            var criteria = _serializer.Serialize(query, JsonPurpose.Storage, false, new[]
            {
                "Identifier",
                "Origin",
                "Filter",
                "Texts"
            });

            var reflector = new Reflector();

            var serialized = new SerializedQuery
            {
                Identifier = id,

                Origin = origin,

                Filter = filter,

                Texts = texts,

                Criteria = criteria,

                Type = query.GetType().Name,

                Class = reflector.GetClassName(query.GetType())
            };

            if (serialized.Class.Length > 200)
                throw new OverflowException($"The assembly-qualified class name for this query ({serialized.Class}) exceeds the maximum character limit (200).");

            if (serialized.Type.Length > 100)
                throw new OverflowException($"The type name for this query ({serialized.Type}) exceeds the maximum character limit (100).");

            return serialized;
        }
    }
}