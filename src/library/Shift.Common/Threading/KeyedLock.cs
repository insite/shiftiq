using System;
using System.Collections.Generic;
using System.Threading;

namespace Shift.Common
{
    public sealed class KeyedLock<TKey>
    {
        #region Classes

        private class LockItem
        {
            public readonly object SyncRoot = new object();
            public int RefCount { get; set; }
        }

        private class LockResult : IDisposable
        {
            private readonly KeyedLock<TKey> _owner;
            private readonly TKey _key;
            private readonly LockItem _item;
            private int _disposed;

            public LockResult(KeyedLock<TKey> owner, TKey key, LockItem item)
            {
                _owner = owner;
                _key = key;
                _item = item;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) != 0)
                    return;

                Monitor.Exit(_item.SyncRoot);
                _owner.ReleaseLock(_key, _item);
            }
        }

        #endregion

        #region Properties

        public int TrackedKeyCount
        {
            get
            {
                lock (_syncRoot)
                    return _items.Count;
            }
        }

        #endregion

        #region Fields

        private readonly Dictionary<TKey, LockItem> _items = new Dictionary<TKey, LockItem>();
        private readonly object _syncRoot = new object();
        private readonly TimeSpan _defaultTimeout = Timeout.InfiniteTimeSpan;

        #endregion

        #region Construction

        public KeyedLock()
        {

        }

        public KeyedLock(TimeSpan defaultTimeout)
        {
            ValidateTimeout(nameof(defaultTimeout), defaultTimeout);

            _defaultTimeout = defaultTimeout;
        }

        #endregion

        #region Lock

        public IDisposable Lock(TKey key)
        {
            return LockInternal(key, _defaultTimeout);
        }

        public IDisposable Lock(TKey key, TimeSpan timeout)
        {
            ValidateTimeout(nameof(timeout), timeout);

            return LockInternal(key, timeout);
        }

        private IDisposable LockInternal(TKey key, TimeSpan timeout)
        {
            var item = AcquireLock(key);
            var acquired = false;

            try
            {
                Monitor.TryEnter(item.SyncRoot, timeout, ref acquired);

                if (!acquired)
                    throw CreateTimeoutException(key, timeout);

                return new LockResult(this, key, item);
            }
            catch
            {
                if (acquired)
                    Monitor.Exit(item.SyncRoot);

                ReleaseLock(key, item);

                throw;
            }
        }

        #endregion

        #region Helper methods

        private LockItem AcquireLock(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            lock (_syncRoot)
            {
                if (!_items.TryGetValue(key, out var item))
                    _items.Add(key, item = new LockItem());

                item.RefCount++;

                return item;
            }
        }

        private void ReleaseLock(TKey key, LockItem item)
        {
            lock (_syncRoot)
            {
                item.RefCount--;

                if (item.RefCount > 0)
                    return;

                _items.Remove(key);
            }
        }

        private static TimeoutException CreateTimeoutException(TKey key, TimeSpan timeout) =>
            new TimeoutException($"Failed to acquire lock for key '{key}' within {timeout}.");

        private static void ValidateTimeout(string paramName, TimeSpan timeout)
        {
            if (timeout != Timeout.InfiniteTimeSpan && timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(paramName, timeout, "Timeout must be positive or Timeout.InfiniteTimeSpan.");
        }

        #endregion
    }
}
