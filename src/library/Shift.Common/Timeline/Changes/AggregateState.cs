using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

using Shift.Common.Timeline.Exceptions;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Represents the state (data) of an aggregate. A derived class should be a POCO (DTO/Packet) that includes a When
    /// method for each event type that changes its property values. Ideally, the property values for an instance of 
    /// this class should be modified only through its When methods.
    /// </summary>
    [Serializable]
    public abstract class AggregateState
    {
        #region Fields

        private readonly IReadOnlyDictionary<Type, Action<object, object>> _whenHandlersInstanceCache;
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, Action<object, object>>> _whenHandlersGlobalCache =
            new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, Action<object, object>>>();

        #endregion

        #region Construction

        public AggregateState()
        {
            _whenHandlersInstanceCache = _whenHandlersGlobalCache.GetOrAdd(GetType(), t => LoadWhenHandlers(t));
        }

        #endregion

        #region Methods (When)

        private IReadOnlyDictionary<Type, Action<object, object>> LoadWhenHandlers(Type stateType)
        {
            var handlers = new Dictionary<Type, Action<object, object>>();
            var methods = stateType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                if (method.Name == "When")
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var paramType = parameters[0].ParameterType;

                        handlers.Add(paramType, BuildWhenCaller(stateType, method, paramType));
                    }
                }
            }

            return new ReadOnlyDictionary<Type, Action<object, object>>(handlers);
        }

        private static Action<object, object> BuildWhenCaller(Type stateType, MethodInfo method, Type paramType)
        {
            // object x
            var paramInstance = Expression.Parameter(typeof(object), "x");

            // object v
            var paramValue = Expression.Parameter(typeof(object), "v");

            // (StateType)x
            var convertInstance = Expression.Convert(paramInstance, stateType);

            // (ParameterType)v
            var convertValue = Expression.Convert(paramValue, paramType);

            // ((StateType)x).When((ParameterType)v)
            var callMethod = Expression.Call(convertInstance, method, convertValue);

            // (object x, object v) => ((StateType)x).When((ParameterType)v)
            var lambda = Expression.Lambda<Action<object, object>>(callMethod, paramInstance, paramValue);

            return lambda.Compile();
        }

        #endregion

        public void Apply(IChange change)
        {
            if (_whenHandlersInstanceCache.TryGetValue(change.GetType(), out var action))
                action(this, change);
            else
                throw new MethodNotFoundException(GetType(), "When", change.GetType());
        }
    }
}
