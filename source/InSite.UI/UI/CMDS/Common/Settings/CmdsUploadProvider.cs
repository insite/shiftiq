using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using Path = System.IO.Path;

namespace InSite.Cmds.Infrastructure
{
    public sealed class CmdsUploadProvider
    {
        #region Constants

        private static readonly Regex AdjustFileNameRegex = new Regex(@"[^a-zA-Z0-9_\- ]+", RegexOptions.Compiled);

        #endregion

        #region Properties

        public static CmdsUploadProvider Current { get; private set; }

        public string StoragePath => _storagePath;

        public string TempStoragePath => _tempStoragePath;

        #endregion

        #region Fields

        private readonly string _storagePath;
        private readonly string _tempStoragePath;
        private readonly ConcurrentDictionary<CmdsUploadKey, CmdsUploadInfo> _cache;
        private static readonly Encoding _utf8 = new UTF8Encoding(false, true);

        #endregion

        #region Construction

        static CmdsUploadProvider()
        {
            Current = new CmdsUploadProvider(
                ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Uploads"),
                ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Temp", "Uploads", "CMDS")
            );
        }

        private CmdsUploadProvider(string storagePath, string tempPath)
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            _storagePath = storagePath;
            _tempStoragePath = tempPath;
            _cache = new ConcurrentDictionary<CmdsUploadKey, CmdsUploadInfo>();
        }

        #endregion

        #region Methods (info)

        public CmdsUploadInfo GetInfo(string containerId, string name)
        {
            if (!Guid.TryParse(containerId, out Guid parsedGuid) || string.IsNullOrEmpty(name))
                return null;

            var key = new CmdsUploadKey(parsedGuid, name);

            return GetInfo(key);
        }

        public CmdsUploadInfo GetInfo(Guid containerId, string name)
        {
            if (containerId == Guid.Empty || string.IsNullOrEmpty(name))
                return null;

            var key = new CmdsUploadKey(containerId, name);

            return GetInfo(key);
        }

        private CmdsUploadInfo GetInfo(CmdsUploadKey key)
        {
            var info = _cache.AddOrUpdate(
                key,
                CreateInfo,
                (k, v) => v.IsExpired ? CreateInfo(k) : v);

            return info.IsNull ? null : info;
        }

        private CmdsUploadInfo CreateInfo(CmdsUploadKey key)
        {
            var data = UploadSearch.BindFirst(
                entity => new
                {
                    entity.ContainerIdentifier,
                    entity.Name,
                    entity.Uploaded,
                },
                x => x.ContainerIdentifier == key.ContainerId && x.Name == key.Name && x.UploadType == UploadType.CmdsFile);

            return data == null
                ? new CmdsUploadInfo()
                : new CmdsUploadInfo(data.ContainerIdentifier, data.Name, data.Uploaded);
        }

        private void OnInfoRemoved(ICmdsUploadModel model)
        {
            var key = new CmdsUploadKey(model.ContainerID, model.Name);

            _cache.AddOrUpdate(
                key,
                k => new CmdsUploadInfo(),
                (k, v) => new CmdsUploadInfo()
             );
        }

        private void OnInfoUpdated(ICmdsUploadModel model)
        {
            var key = new CmdsUploadKey(model.ContainerID, model.Name);

            _cache.AddOrUpdate(
                key,
                CreateInfo,
                (k, v) => v.Timestamp < Clock.Trim(model.Uploaded) ? CreateInfo(k) : v);
        }

        #endregion

        #region Methods (file)

        public Stream Read(Guid containerId, string name)
        {
            var info = GetInfo(containerId, name);

            return Read(info);
        }

        public Stream Read(CmdsUploadInfo info)
        {
            if (info != null && !info.IsNull)
            {
                var filePath = Path.Combine(_storagePath, info.ContainerIdentifier.ToString(), info.Name);
                var isException = true;

                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        if (File.Exists(filePath))
                            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                        isException = false;

                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }

                if (!isException)
                    info.Expire();
            }

            return Stream.Null;
        }

        public ICmdsUploadModel Save(string containerType, Guid containerId, string name, string data) =>
            Save(containerType, containerId, name, data, _utf8);

        public ICmdsUploadModel Save(string containerType, Guid containerId, string name, string data, Encoding encoding)
        {
            return Save(containerType, containerId, name, stream =>
            {
                var writer = new StreamWriter(stream, encoding);
                writer.Write(data);
                writer.Flush();
            });
        }

        public ICmdsUploadModel Save(string containerType, Guid containerId, string name, byte[] data)
        {
            return Save(containerType, containerId, name, stream =>
            {
                var writer = new BinaryWriter(stream);
                writer.Write(data);
                writer.Flush();
            });
        }

        public ICmdsUploadModel Save(string containerType, Guid containerId, string name, Stream stream) =>
            Update(containerType, containerId, name, model => model.Write(stream));

        public ICmdsUploadModel Save(string containerType, Guid containerId, string name, Action<Stream> write) =>
            Update(containerType, containerId, name, model => model.Write(write));

        public ICmdsUploadModel Update(string containerType, Guid containerId, string name, Action<ICmdsUploadModel> action) => Update((CmdsUploadStorage storage) =>
        {
            var model = storage.Open(containerId, name, containerType);

            if (model.IsNew)
                model.Title = name;

            action(model);

            return model;
        });

        public ICmdsUploadModel Update(Guid uploadId, Action<ICmdsUploadModel> action) => Update((CmdsUploadStorage storage) =>
        {
            var model = storage.Open(uploadId);

            action(model);

            return model;
        });

        private ICmdsUploadModel Update(Func<CmdsUploadStorage, ICmdsUploadModel> update)
        {
            using (var storage = CreateFileStorage())
            {
                var model = update(storage);

                if (model != null)
                {
                    model.Save();

                    storage.Commit();
                }

                return model;
            }
        }

        public ICmdsUploadModel Move(string sourceGuid, string sourceName, string destinationGuid, string destinationName, string containerType) =>
            Move(Guid.Parse(sourceGuid), sourceName, Guid.Parse(destinationGuid), destinationName, containerType);

        public ICmdsUploadModel Move(Guid sourceContainerId, string sourceName, Guid destinationContainerId, string destinationName, string containerType)
        {
            using (var storage = CreateFileStorage())
            {
                var model = storage.Open(sourceContainerId, sourceName, null);
                if (model.IsNew)
                    return null;

                model.ContainerID = destinationContainerId;
                model.Name = destinationName;
                model.ContainerType = containerType;
                model.Save();

                storage.Commit();

                return model;
            }
        }

        public ICmdsUploadModel Copy(string sourceContainerId, string sourceName, string destinationContainerId, string destinationName) =>
            Copy(Guid.Parse(sourceContainerId), sourceName, Guid.Parse(destinationContainerId), destinationName);

        public ICmdsUploadModel Copy(Guid sourceContainerId, string sourceName, Guid destinationContainerId, string destinationName)
        {
            using (var storage = CreateFileStorage())
            {
                ICmdsUploadModel destinationModel = null;

                var sourceModel = storage.Open(sourceContainerId, sourceName, null);

                if (!sourceModel.IsNew)
                {
                    using (var sourceStream = sourceModel.Read())
                    {
                        if (sourceStream != Stream.Null)
                        {
                            destinationModel = storage.Open(destinationContainerId, destinationName, sourceModel.ContainerType);

                            if (destinationModel.IsNew)
                            {
                                destinationModel.Title = sourceModel.Title;
                                destinationModel.Description = sourceModel.Description;
                            }

                            destinationModel.Write(sourceStream);
                            destinationModel.Save();
                        }
                    }

                    if (destinationModel != null)
                        storage.Commit();
                }

                return destinationModel;
            }
        }

        public void Delete(Guid containerId, string name)
        {
            using (var storage = CreateFileStorage())
            {
                Delete(containerId, name, storage);

                storage.Commit();
            }
        }

        public static void Delete(Guid containerId, string name, CmdsUploadStorage storage)
        {
            var file = storage.Open(containerId, name, null);
            if (file.IsNew)
                return;

            file.Delete();
        }

        public void Delete(Guid uploadId)
        {
            using (var storage = CreateFileStorage())
            {
                Delete(uploadId, storage);

                storage.Commit();
            }
        }

        public static void Delete(Guid uploadId, CmdsUploadStorage storage)
        {
            var file = storage.Open(uploadId);
            if (file == null)
                return;

            file.Delete();
        }

        #endregion

        #region Helper methods

        public CmdsUploadStorage CreateFileStorage()
        {
            var storage = new CmdsUploadStorage(this);
            storage.EntityUpdated += OnInfoUpdated;
            storage.EntityRemoved += OnInfoRemoved;
            return storage;
        }

        public static string GetFileRelativeUrl(Upload entity)
        {
            return GetFileRelativeUrl(entity.ContainerIdentifier, entity.Name);
        }

        public static string GetFileRelativeUrl(Guid containerId, string name)
        {
            return $"/cmds/uploads/{containerId}/{Uri.EscapeDataString(name)}";
        }

        public static string AdjustFileName(string originalFileName)
        {
            string fileName = Path.GetFileNameWithoutExtension(originalFileName);
            string fileExtension = Path.GetExtension(originalFileName);

            return AdjustFileNameRegex.Replace(fileName, "-") + fileExtension;
        }

        #endregion
    }
}