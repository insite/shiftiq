using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

using InSite.Common.Web;

namespace InSite.Web.Infrastructure
{
    public static class TempFileStorage
    {
        #region Classes

        private class DirLock
        {
            public string Path { get; set; }
            public bool Exists { get; set; } = true;
        }

        #endregion

        #region Fields

        private const int TimerDueTime = 1 * 60 * 1000; // milliseconds
        private const int FileLifetime = 1 * 60; // minutes

        private static readonly object _syncRoot = new object();
        private static readonly ConcurrentDictionary<Guid, DirLock> _dirLocks = new ConcurrentDictionary<Guid, DirLock>();

        private static readonly string _storagePath;
        private static Timer _timer;

        #endregion

        #region Construction

        static TempFileStorage()
        {
            _storagePath = Path.Combine(ServiceLocator.FilePaths.TempFolderPath, "Storage");

            var dir = new DirectoryInfo(_storagePath);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                    file.Delete();

                foreach (var subDir in dir.GetDirectories())
                    subDir.Delete(true);
            }
            else
            {
                dir.Create();
            }
        }

        #endregion

        #region Methods (IO)

        public static Guid Create()
        {
            var dirId = Guid.NewGuid();
            var dLock = _dirLocks.GetOrAdd(dirId, CreateDirLock);

            lock (dLock)
            {
                var dir = new DirectoryInfo(dLock.Path);

                if (!dir.Exists)
                    dir.Create();

                Directory.SetLastAccessTime(dir.FullName, DateTime.Now);

                dLock.Exists = true;
            }

            EnsureTimerExists();

            return dirId;
        }

        public static void Open(Guid id, Action<DirectoryInfo> action)
        {
            var dLock = _dirLocks.GetOrAdd(id, CreateDirLock);

            lock (dLock)
            {
                if (dLock.Exists)
                {
                    var dir = new DirectoryInfo(dLock.Path);

                    try
                    {
                        action(dir);
                    }
                    catch (FileStorage.MaxFileSizeExceededException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        AppSentry.SentryError(ex);
                    }
                    finally
                    {
                        if (dir.Exists)
                            Directory.SetLastAccessTime(dir.FullName, DateTime.Now);
                    }
                }
            }

            EnsureTimerExists();
        }

        public static bool Exists(Guid id)
        {
            var result = false;
            var dLock = _dirLocks.GetOrAdd(id, CreateDirLock);

            lock (dLock)
            {
                var dir = new DirectoryInfo(dLock.Path);
                if (dir.Exists)
                {
                    Directory.SetLastAccessTime(dir.FullName, DateTime.Now);
                    result = true;
                }
            }

            EnsureTimerExists();

            return result;
        }

        public static void Remove(Guid id)
        {
            var dLock = _dirLocks.GetOrAdd(id, CreateDirLock);

            lock (dLock)
            {
                if (!dLock.Exists)
                    return;

                var dir = new DirectoryInfo(dLock.Path);
                if (dir.Exists)
                    dir.Delete(true);

                dLock.Exists = false;
            }

            EnsureTimerExists();
        }

        public static bool CleanUp()
        {
            var isClean = true;

            lock (_syncRoot)
            {
                var rootDir = new DirectoryInfo(_storagePath);

                foreach (var subDir in rootDir.GetDirectories())
                {
                    if (Guid.TryParse(Path.GetFileNameWithoutExtension(subDir.Name), out var id))
                    {
                        var dLock = _dirLocks.GetOrAdd(id, CreateDirLock);

                        lock (dLock)
                        {
                            var accessDate = subDir.LastAccessTime;

                            if (accessDate < subDir.LastWriteTime)
                                accessDate = subDir.LastWriteTime;

                            dLock.Exists = (DateTime.Now - accessDate).TotalMinutes <= FileLifetime;
                        }

                        if (!dLock.Exists)
                        {
                            subDir.Delete(true);
                            _dirLocks.TryRemove(id, out _);
                        }
                        else
                        {
                            isClean = false;
                        }
                    }
                    else
                    {
                        subDir.Delete(true);
                    }
                }

                foreach (var kv in _dirLocks)
                {
                    if (!kv.Value.Exists)
                        _dirLocks.TryRemove(kv.Key, out _);
                }
            }

            return isClean;
        }

        private static void EnsureTimerExists()
        {
            lock (_syncRoot)
            {
                if (_timer == null)
                    _timer = new Timer(OnTimer, null, TimerDueTime, Timeout.Infinite);
            }
        }

        private static void OnTimer(object state)
        {
            lock (_syncRoot)
            {
                if (CleanUp())
                {
                    try
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    _timer.Change(TimerDueTime, Timeout.Infinite);
                }
            }
        }

        #endregion

        #region Methods (helpers)

        private static string GetDirPath(Guid key) =>
            Path.Combine(_storagePath, key.ToString());

        private static DirLock CreateDirLock(Guid key)
        {
            var path = GetDirPath(key);

            return new DirLock
            {
                Path = path,
                Exists = Directory.Exists(path)
            };
        }

        #endregion
    }
}