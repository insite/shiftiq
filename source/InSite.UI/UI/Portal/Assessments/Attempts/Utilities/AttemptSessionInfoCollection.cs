using System;
using System.Collections.Concurrent;

using InSite.Domain.Attempts;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AttemptSessionInfoCollection
    {
        #region Enums

        public enum UpdateStatus
        {
            Extended,
            Updated,
            Rejected
        }

        #endregion

        #region Fields

        private DateTime _nextClearTime = DateTime.UtcNow.AddSeconds(AttemptConfiguration.DefaultPingInterval * 2);
        private ConcurrentDictionary<Guid, AttemptSessionInfo> _items = new ConcurrentDictionary<Guid, AttemptSessionInfo>();
        private readonly object _syncRoot = new object();

        #endregion

        #region Methods

        public UpdateStatus Update(Guid attemptId, string sessionId)
        {
            UpdateStatus result;
            var info = _items.GetOrAdd(attemptId, x => new AttemptSessionInfo(sessionId));

            lock (info.SyncRoot)
            {
                if (info.SessionId == sessionId)
                {
                    info.Update();
                    result = UpdateStatus.Extended;
                }
                else if ((DateTime.UtcNow - info.Timestamp).TotalSeconds > AttemptConfiguration.DefaultPingInterval * 2)
                {
                    info.Update(sessionId);
                    result = UpdateStatus.Updated;
                }
                else
                {
                    result = UpdateStatus.Rejected;
                }
            }

            if (info.IsRemoved)
                return Update(attemptId, sessionId);

            Clear();

            return result;
        }

        private void Clear()
        {
            if (DateTime.UtcNow <= _nextClearTime)
                return;

            lock (_syncRoot)
            {
                if (DateTime.UtcNow <= _nextClearTime)
                    return;

                var untilTimestamp = DateTime.UtcNow.AddMinutes(-1);

                foreach (var item in _items)
                {
                    if (item.Value.Timestamp > untilTimestamp)
                        continue;

                    lock (item.Value.SyncRoot)
                    {
                        if (item.Value.Timestamp <= untilTimestamp)
                        {
                            _items.TryRemove(item.Key, out var value);
                            item.Value.Remove();
                        }
                    }
                }

                _nextClearTime = DateTime.UtcNow.AddSeconds(AttemptConfiguration.DefaultPingInterval * 2);
            }
        }

        #endregion
    }
}