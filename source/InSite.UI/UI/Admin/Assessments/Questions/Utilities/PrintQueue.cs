using System;
using System.IO;
using System.Web.Hosting;

using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public static class PrintQueue
    {
        #region Fields

        private static readonly string _rootStoragePath;
        private static readonly object _syncRoot = new object();

        #endregion

        #region Construction

        static PrintQueue()
        {
            _rootStoragePath = Path.Combine(ServiceLocator.FilePaths.TempFolderPath, nameof(PrintQueue));

            InitStorage(_rootStoragePath);
        }

        private static void InitStorage(string path)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.EnumerateFiles())
                    file.Delete();
            }
            else
            {
                dir.Create();
            }
        }

        #endregion

        #region Methods (public)

        public static string GetStoragePath(string name)
        {
            var path = Path.Combine(_rootStoragePath, name);

            InitStorage(path);

            return path;
        }

        public static bool IsLocked(string storagePath, Guid user) => IsStorageLocked(user);

        public static void QueuePrint<TOptions>(string storagePath, Guid user, Guid entity, TOptions options, Func<Guid, TOptions, PrintOutputFile> print)
        {
            if (!TryLockStorage(user))
                return;

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    var file = print(user, options);
                    if (file != null)
                        SaveDone(storagePath, user, entity, file);
                }
                finally
                {
                    UnlockStorage(user);
                }
            });
        }

        public static PrintOutputFile GetPrintFile(string storagePath, Guid user, Guid entity)
        {
            if (TryLockStorage(user))
            {
                try
                {
                    var result = LoadDone(storagePath, user, entity);

                    DeleteDone(storagePath, user, entity);

                    return result;
                }
                finally
                {
                    UnlockStorage(user);
                }
            }

            return null;
        }

        #endregion

        #region Methods (done)

        private static void DeleteDone(string storagePath, Guid user, Guid entity)
        {
            var filePath = GetDonePath(storagePath, user, entity);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private static void SaveDone(string storagePath, Guid user, Guid entity, PrintOutputFile file)
        {
            var filePath = GetDonePath(storagePath, user, entity);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var stream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(file.Name);
                    writer.Write(file.Data.Length);
                    writer.Write(file.Data);
                }
            }
        }

        private static PrintOutputFile LoadDone(string storagePath, Guid user, Guid entity)
        {
            var filePath = GetDonePath(storagePath, user, entity);

            if (File.Exists(filePath))
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var fileName = reader.ReadString();
                        var dataLength = reader.ReadInt32();
                        var dataBytes = reader.ReadBytes(dataLength);

                        return new PrintOutputFile(fileName, dataBytes);
                    }
                }
            }

            return null;
        }

        private static string GetDonePath(string storagePath, Guid user, Guid entity)
        {
            return Path.Combine(storagePath, $"{user}.{entity}.done");
        }

        #endregion

        #region Methods (lock)

        private static bool IsStorageLocked(Guid user)
        {
            var path = GetLockPath(user);

            lock (_syncRoot)
                return File.Exists(path);
        }

        private static bool TryLockStorage(Guid user)
        {
            var path = GetLockPath(user);

            lock (_syncRoot)
            {
                if (File.Exists(path))
                    return false;

                using (var stream = File.Create(path))
                    return true;
            }
        }

        private static void UnlockStorage(Guid user)
        {
            var path = GetLockPath(user);

            lock (_syncRoot)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        private static string GetLockPath(Guid user)
        {
            return Path.Combine(_rootStoragePath, user.ToString() + ".lock");
        }

        #endregion
    }
}