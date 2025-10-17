using System;
using System.Collections.Generic;
using System.Threading;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public static class CmdsUploadQueue
    {
        #region Classes

        private class KeyInfo
        {
            #region Properties

            public CmdsUploadKey Key { get; }

            public bool IsEmpty => _storageQueue.First == null;

            #endregion

            #region Fields

            private LinkedList<Guid> _storageQueue = new LinkedList<Guid>();

            #endregion

            #region Construction

            public KeyInfo(CmdsUploadKey key)
            {
                Key = key;
            }

            #endregion

            #region Methods

            public bool IsLockedBy(Guid storageId)
            {
                return _storageQueue.First.Value == storageId;
            }

            public void Enqueue(Guid storageId)
            {
                if (!_storageQueue.Contains(storageId))
                    _storageQueue.AddLast(storageId);
            }

            public void Dequeue()
            {
                _storageQueue.RemoveFirst();
            }

            public void Dequeue(Guid storageId)
            {
                _storageQueue.Remove(storageId);
            }

            #endregion
        }

        private class StorageInfo
        {
            #region Properties

            public Guid StorageId { get; }
            public List<KeyInfo> KeyList { get; } = new List<KeyInfo>();

            #endregion

            #region Construction

            public StorageInfo(Guid storageId)
            {
                StorageId = storageId;
            }

            #endregion

            #region Methods

            public bool AllowExecute()
            {
                for (var i = 0; i < KeyList.Count; i++)
                {
                    if (!KeyList[i].IsLockedBy(StorageId))
                        return false;
                }

                return true;
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<CmdsUploadKey, KeyInfo> _keys = new Dictionary<CmdsUploadKey, KeyInfo>();

        #endregion

        public static void Execute(Guid storageId, IEnumerable<CmdsUploadKey> uploadKeys, Action action)
        {
            var storageInfo = new StorageInfo(storageId);

            try
            {
                lock (_syncRoot)
                {
                    foreach (var uploadKey in uploadKeys)
                    {
                        KeyInfo keyInfo;

                        if (_keys.ContainsKey(uploadKey))
                            keyInfo = _keys[uploadKey];
                        else
                            _keys.Add(uploadKey, keyInfo = new KeyInfo(uploadKey));

                        storageInfo.KeyList.Add(keyInfo);
                        keyInfo.Enqueue(storageId);
                    }
                }

                var tryCount = 1000;

                while (true)
                {
                    if (storageInfo.AllowExecute())
                    {
                        action();

                        lock (_syncRoot)
                        {
                            for (var i = 0; i < storageInfo.KeyList.Count; i++)
                            {
                                var keyInfo = storageInfo.KeyList[i];

                                keyInfo.Dequeue();

                                if (keyInfo.IsEmpty)
                                    _keys.Remove(keyInfo.Key);
                            }

                            storageInfo.KeyList.Clear();
                        }

                        break;
                    }

                    if (tryCount-- <= 0)
                        throw new ApplicationError("Unable to lock files: " + string.Join(", ", uploadKeys));

                    Thread.Sleep(5);
                }
            }
            catch
            {
                lock (_syncRoot)
                {
                    foreach (var uploadKey in uploadKeys)
                    {
                        if (!_keys.ContainsKey(uploadKey))
                            continue;

                        var keyInfo = _keys[uploadKey];

                        keyInfo.Dequeue(storageId);

                        if (keyInfo.IsEmpty)
                            _keys.Remove(uploadKey);
                    }
                }

                throw;
            }
        }
    }
}