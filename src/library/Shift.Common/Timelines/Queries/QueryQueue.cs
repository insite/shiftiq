using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Exceptions;
using Shift.Common.Timeline.Services;

using Shift.Common;

namespace Shift.Common.Timeline.Queries
{
    /// <summary>
    /// Implements a basic query queue. The purpose of the queue is to route queries to handler methods; validation of
    /// a query itself is the responsibility of its handler.
    /// </summary>
    public class QueryQueue : IQueryQueue
    {
        private readonly IJsonSerializer _serializer;

        /// <summary>
        /// A query's full class name is the key to find the method that handles it.
        /// </summary>
        private readonly Dictionary<string, Delegate> _handlers = new Dictionary<string, Delegate>();

        public QueryQueue()
        {
            _serializer = ServiceLocator.Instance.GetService<IJsonSerializer>();
        }

        private TResult Execute<TResult>(IQuery<TResult> query, string @class)
        {
            if (_handlers.ContainsKey(@class))
            {
                var action = _handlers[@class];
                return (TResult)action.DynamicInvoke(query);
            }

            return default;
        }

        public TResult Send<TResult>(IQuery<TResult> query)
        {
            return Execute(query, _serializer.GetClassName(query.GetType()));
        }

        public void Subscribe<TQuery, TResult>(Func<TQuery, TResult> handle) where TQuery : IQuery<TResult>
        {
            var name = _serializer.GetClassName(typeof(TQuery));

            if (_handlers.Any(x => x.Key == name))
                throw new AmbiguousQueryHandlerException(name);

            _handlers.Add(name, handle);
        }
    }
}