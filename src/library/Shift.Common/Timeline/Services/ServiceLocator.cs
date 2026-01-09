using System;
using System.Collections.Generic;

namespace Shift.Common.Timeline.Services
{
    /// <summary>
    /// Provides a simple service locator for helpers required by the Timeline library. By design, the Timeline library
    /// imposes no dependencies on any other libraries to maximize its testability and reusability.
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator _locator = null;
        
        private readonly Dictionary<Type, object> _registry = new Dictionary<Type, object>();

        public static ServiceLocator Instance
        {
            get
            {
                if (_locator == null)
                    _locator = new ServiceLocator();
                
                return _locator;
            }
        }

        private ServiceLocator() { }

        public void Register<T>(T serviceInstance)
        {
            _registry[typeof(T)] = serviceInstance;
        }

        public T GetService<T>()
        {
            if (!_registry.ContainsKey(typeof(T)))
                throw new Exception(string.Format("You must register an instance of type {0} with the Shift.Common.Timeline service locator.", typeof(T).Name));

            T serviceInstance = (T)_registry[typeof(T)];
            return serviceInstance;
        }
    }
}