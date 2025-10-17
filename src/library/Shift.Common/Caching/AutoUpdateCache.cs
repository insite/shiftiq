using System;

namespace Shift.Common
{
    public abstract class AutoUpdateCache
    {
        #region Fields

        private readonly object _syncRoot;
        private readonly TimeSpan _updatePeriod;

        private DateTime _nextUpdateAt;

        #endregion

        #region Construction

        public AutoUpdateCache(TimeSpan updatePeriod)
        {
            _syncRoot = new object();
            _updatePeriod = updatePeriod;
            _nextUpdateAt = DateTime.MinValue;
        }

        #endregion

        #region Methods

        protected void EnsureUpdated()
        {
            if (DateTime.UtcNow >= _nextUpdateAt)
            {
                lock (_syncRoot)
                {
                    if (DateTime.UtcNow >= _nextUpdateAt)
                    {
                        Update();

                        _nextUpdateAt = DateTime.UtcNow.Add(_updatePeriod);
                    }
                }
            }
        }

        protected abstract void Update();

        #endregion
    }

    public class AutoUpdateCache<TData> : AutoUpdateCache
    {
        #region Fields

        private readonly Func<TData> _dataFactory;
        private TData _data;

        #endregion

        #region Construction

        public AutoUpdateCache(TimeSpan updatePeriod, Func<TData> dataFactory)
            : base(updatePeriod)
        {
            _dataFactory = dataFactory ?? throw new ArgumentNullException(nameof(updatePeriod));
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            _data = _dataFactory();
        }

        public TData GetData()
        {
            EnsureUpdated();

            return _data;
        }

        #endregion
    }

    public class AutoUpdateCache<TParam, TData> : AutoUpdateCache
    {
        #region Fields

        private readonly Func<TParam, TData> _dataFactory;
        private readonly TParam _factoryParams;
        private TData _data;

        #endregion

        #region Construction

        public AutoUpdateCache(TimeSpan updatePeriod, TParam factoryParams, Func<TParam, TData> dataFactory)
            : base(updatePeriod)
        {
            if (factoryParams == null)
                throw new ArgumentNullException(nameof(factoryParams));

            _factoryParams = factoryParams;
            _dataFactory = dataFactory ?? throw new ArgumentNullException(nameof(updatePeriod));
        }

        #endregion

        #region Methods

        protected override void Update() => _data = _dataFactory(_factoryParams);

        public TData GetData()
        {
            EnsureUpdated();

            return _data;
        }

        #endregion
    }
}
