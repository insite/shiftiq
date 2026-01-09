using System;
using System.Collections.Concurrent;
using System.Web.Hosting;

namespace InSite.Web.Instrumentation
{
    public static class LongRunningTask
    {
        private static readonly ConcurrentDictionary<string, object> _taskIds = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public static bool Create(string name, Action action)
        {
            if (!_taskIds.TryAdd(name, null))
                return false;

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    action();
                }
                finally
                {
                    _taskIds.TryRemove(name, out _);
                }
            });

            return true;
        }

        public static bool Create<TData>(string name, TData data, Action<TData> action) where TData : class
        {
            if (!_taskIds.TryAdd(name, data))
                return false;

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    action(data);
                }
                finally
                {
                    _taskIds.TryRemove(name, out _);
                }
            });

            return true;
        }

        public static bool Create<TData>(string name, Func<TData> dataFactory, Action<TData> action) where TData : class
        {
            TData data;
            if (_taskIds.ContainsKey(name) || !_taskIds.TryAdd(name, data = dataFactory()))
                return false;

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    action(data);
                }
                finally
                {
                    _taskIds.TryRemove(name, out _);
                }
            });

            return true;
        }

        public static bool TryGetData<TData>(string name, out TData data) where TData : class
        {
            var exists = _taskIds.TryGetValue(name, out var obj);

            data = exists ? obj as TData : default;

            return exists;
        }

        public static bool Exists(string name)
        {
            return _taskIds.ContainsKey(name);
        }
    }
}