using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using TimelineServices = Shift.Common.Timeline.Services;

namespace Shift.Common
{
    /// <summary>
    /// This is a generic cache based on key/value pairs, where both key and value are generic. Keys must be unique. Every
    /// entry in the cache has its own timeout. Cache is a thread-safe class, and it deletes expired entries on its own 
    /// using System.Threading.Timers that run on <see cref="ThreadPool"/> threads.
    /// </summary>
    public class MemoryCache<TKey, TData> : IDisposable, IMemoryCache<TKey, TData>
    {
        #region Events

        public event MemoryCacheHandler<TKey, TData> ItemAdded;

        private void OnItemAdded(TKey key, TData data) => ItemAdded?.Invoke(this, new MemoryCacheArgs<TKey, TData>(key, data));

        public event MemoryCacheHandler<TKey, TData> ItemRemoved;

        private void OnItemRemoved(TKey key, TData data) => ItemRemoved?.Invoke(this, new MemoryCacheArgs<TKey, TData>(key, data));

        #endregion

        #region Methods (construction)

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache{TK,T}"/> class.
        /// </summary>
        public MemoryCache() { }

        private readonly Dictionary<TKey, TData> _cache = new Dictionary<TKey, TData>();
        private readonly Dictionary<TKey, Timer> _timers = new Dictionary<TKey, Timer>();
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        #endregion

        #region Methods (destruction)

        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    // Dispose managed resources.
                    Clear();
                    _locker.Dispose();
                }

                // Dispose unmanaged resources
            }
        }

        /// <summary>
        /// Clears the cache and disposes all active timers.
        /// </summary>
        public void Clear()
        {
            Tuple<TKey, TData>[] items;

            _locker.EnterWriteLock();

            try
            {
                items = _cache
                    .Select(kv => new Tuple<TKey, TData>(kv.Key, kv.Value))
                    .ToArray();

                foreach (var t in _timers.Values)
                {
                    try
                    {
                        t.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                _timers.Clear();
                _cache.Clear();
            }
            finally
            {
                _locker.ExitWriteLock();
            }

            foreach (var item in items)
                OnItemRemoved(item.Item1, item.Item2);
        }

        #endregion

        #region Methods (write - timer)

        /// <summary>
        /// Check if a specific timer already exists. If not then add a new one.
        /// </summary>
        private void CheckTimer(TKey key, int cacheTimeout, bool restartTimerIfExists)
        {
            if (_timers.TryGetValue(key, out var timer))
            {
                if (restartTimerIfExists)
                {
                    timer.Change(
                        (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
                        Timeout.Infinite);
                }
            }
            else
                _timers.Add(
                    key,
                    new Timer(
                        RemoveByTimer,
                        key,
                        (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
                        Timeout.Infinite));
        }

        private void RemoveByTimer(object state)
        {
            Remove((TKey)state);
        }

        #endregion

        #region Methods (write)

        /// <summary>
        /// Adds or updates the specified cache-key with the specified cacheObject and applies a specified timeout (in seconds) to this key.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        /// <param name="data">The cache object to store.</param>
        /// <param name="cacheTimeout">The lifespan of this object in seconds. Must be 1 or greater. Use 
        /// Timeout.Infinite to keep the entry forever.</param>
        /// <param name="restartTimerIfExists">(Optional). If <c>true</c> then reset the timer for this cacheObject even 
        /// if the object already exists in the cache.</param>
        public void Add(TKey key, TData data, int cacheTimeout, bool restartTimerIfExists = false)
        {
            if (_disposed)
                return;

            if (cacheTimeout != Timeout.Infinite && cacheTimeout < 1)
                throw new ArgumentOutOfRangeException(nameof(cacheTimeout));

            _locker.EnterWriteLock();

            try
            {
                CheckTimer(key, cacheTimeout, restartTimerIfExists);

                if (!_cache.ContainsKey(key))
                    _cache.Add(key, data);
                else
                    _cache[key] = data;
            }
            finally
            {
                _locker.ExitWriteLock();
            }

            OnItemAdded(key, data);
        }

        /// <summary>
        /// Adds or updates the specified cache-key with the specified cacheObject and applies <c>Timeout.Infinite</c> to this key.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        /// <param name="cacheObject">The cache object to store.</param>
        public void Add(TKey key, TData cacheObject)
        {
            Add(key, cacheObject, Timeout.Infinite);
        }

        /// <summary>
        /// Removes a series of cache entries in a single call for all key that match the specified key pattern.
        /// </summary>
        /// <param name="keyPattern">The key pattern to remove. The Predicate has to return true to get key removed.</param>
        public void Remove(Predicate<TKey> keyPattern)
        {
            if (_disposed)
                return;

            Tuple<TKey, TData>[] items;

            _locker.EnterWriteLock();

            try
            {
                items = _cache
                    .Where(kv => keyPattern(kv.Key))
                    .Select(kv => new Tuple<TKey, TData>(kv.Key, kv.Value))
                    .ToArray();

                foreach (var item in items)
                {
                    try
                    {
                        _timers[item.Item1].Dispose();
                    }
                    catch
                    {
                        // ignored
                    }

                    _timers.Remove(item.Item1);
                    _cache.Remove(item.Item1);
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }

            foreach (var item in items)
                OnItemRemoved(item.Item1, item.Item2);
        }

        /// <summary>
        /// Removes the specified cache entry with the specified key.
        /// If the key is not found, no exception is thrown, the statement is just ignored.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        public void Remove(TKey key)
        {
            if (_disposed)
                return;

            TData data;

            _locker.EnterWriteLock();

            try
            {
                if (!_cache.TryGetValue(key, out data))
                    return;

                try
                {
                    _timers[key].Dispose();
                }
                catch
                {
                    // ignored
                }

                _timers.Remove(key);
                _cache.Remove(key);

            }
            finally
            {
                _locker.ExitWriteLock();
            }

            OnItemRemoved(key, data);
        }

        #endregion

        #region Methods (read)

        /// <summary>
        /// Gets the cache entry with the specified key or returns <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        /// <returns>The object from the cache or <c>default(T)</c>, if not found.</returns>
        public TData this[TKey key] => Get(key);

        /// <summary>
        /// Gets the value for a specific key.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        /// <returns>The corresponding value from the cache if it exists, otherwise <c>default(T)</c>.</returns>
        public TData Get(TKey key)
        {
            if (_disposed)
                return default;

            _locker.EnterReadLock();

            try
            {
                return _cache.TryGetValue(key, out var rv) ? rv : default;
            }

            finally
            {
                _locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets all values.
        /// </summary>
        /// <returns>The all values from the cache if it exists, otherwise empty array.</returns>
        public TData[] GetAll()
        {
            if (_disposed)
                return default;

            _locker.EnterReadLock();

            try
            {
                return _cache.Values.ToArray();
            }

            finally
            {
                _locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Tries to get the value for a specific key.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        /// <param name="value">(out) The value, if found, otherwise <c>default(T)</c>.</param>
        /// <returns><c>True</c>, if <c>key</c> exists, otherwise <c>False</c>.</returns>
        public bool TryGet(TKey key, out TData value)
        {
            if (_disposed)
            {
                value = default;

                return false;
            }

            _locker.EnterReadLock();

            try
            {
                return _cache.TryGetValue(key, out value);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Checks if a specific key exists in the cache.
        /// </summary>
        /// <param name="key">The key for a cached value.</param>
        /// <returns><c>True</c> if the key exists in the cache, otherwise <c>False</c>.</returns>
        public bool Exists(TKey key)
        {
            if (_disposed)
                return false;

            _locker.EnterReadLock();

            try
            {
                return _cache.ContainsKey(key);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        #endregion
    }

    #region Classes (derived)

    /// <summary>
    /// This is a generic cache based on key/value pairs where key is a string. You can add any item to this cache as 
    /// long as the key is unique, so treat keys like namespaces and apply consistent naming conventions throughout 
    /// your application. Every cache entry has its own timeout. This class is thread safe and will delete expired 
    /// entries on its own using System.Threading.Timers (which run on <see cref="ThreadPool"/> threads).
    /// </summary>
    public class MemoryCache<T> : MemoryCache<string, T>
    {

    }

    public class GuidCache<T> : MemoryCache<Guid, T>, TimelineServices.IGuidCache<T>
    {

    }

    /// <summary>
    /// The non-generic Cache class instanciates a Cache{object} that can be used with any type of (mixed) content. It 
    /// also publishes a static <c>.Global</c> member, so a cache can be used without creating a dedicated instance. 
    /// The <c>.Global</c> member is lazy-instantiated.
    /// </summary>
    public class Cache : MemoryCache<string, object>
    {
        private static readonly Lazy<Cache> Lazy = new Lazy<Cache>();

        /// <summary>
        /// Gets the global shared cache instance valid for the entire process.
        /// </summary>
        public static Cache Global => Lazy.Value;
    }

    #endregion
}
