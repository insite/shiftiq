using System;
using System.Collections.Generic;

namespace Shift.Common
{
    public class QueryRunner : IQueryRunner
    {
        private readonly Dictionary<Type, Func<object, object>> _methods;

        public QueryRunner()
        {
            _methods = new Dictionary<Type, Func<object, object>>();
        }

        public bool CanRun(Type queryType)
        {
            return _methods.ContainsKey(queryType);
        }

        public TResult Run<TResult>(IQuery<TResult> query)
        {
            if (_methods.TryGetValue(query.GetType(), out var runner))
            {
                var result = runner(query);
                return (TResult)result;
            }

            throw new InvalidOperationException($"Missing query runner method for {query.GetType().Name}");
        }

        protected void RegisterQuery<T>(Func<object, object> queryMethod)
        {
            _methods.Add(typeof(T), queryMethod);
        }
    }
}