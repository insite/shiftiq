using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    /// <summary>
    /// Implements a basic queue for running queries.
    /// </summary>
    /// <remarks>
    /// The purpose of the queue is to route queries to handler methods. Query validation is the
    /// responsibility of its subscriber.
    /// </remarks>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IEnumerable<IQueryRunner> _runners;

        public QueryDispatcher(IEnumerable<IQueryRunner> runners)
        {
            _runners = runners;
        }

        public TResult Dispatch<TResult>(IQuery<TResult> query)
        {
            var runner = _runners.FirstOrDefault(h => h.CanRun(query.GetType()));

            return runner != null
                ? runner.Run(query)
                : throw new InvalidOperationException($"Missing query runner class for {query.GetType().Name}.");
        }
    }
}