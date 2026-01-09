using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using InSite.Persistence;

namespace InSite.Common.Web
{
    public sealed class FileProvider
    {
        #region Properties

        public Guid UserIdentifier => _getUserIdentifier();
        public string UserName => _getUserName();

        public string StoragePath => _storagePath;

        #endregion

        #region Fields

        private readonly string _storagePath;

        private readonly Func<Guid> _getUserIdentifier;
        private readonly Func<string> _getUserName;

        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, FileDescriptor>> _organizations;
        private readonly ConcurrentDictionary<string, FileDescriptor> _paths;

        public static readonly Encoding UTF8 = new UTF8Encoding(false, true);

        #endregion

        #region Construction

        public FileProvider(string storagePath, Func<string> getUserName, Func<Guid> getUserIdentifier)
        {
            if (string.IsNullOrEmpty(storagePath))
                throw new ArgumentNullException(nameof(storagePath));

            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            _storagePath = storagePath;
            _getUserName = getUserName ?? throw new ArgumentNullException(nameof(getUserName));
            _getUserIdentifier = getUserIdentifier ?? throw new ArgumentNullException(nameof(getUserIdentifier));
            _organizations = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, FileDescriptor>>();
            _paths = new ConcurrentDictionary<string, FileDescriptor>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Descriptor Management

        private ConcurrentDictionary<string, FileDescriptor> CreateDescriptorsStorage(Guid organizationId) =>
            new ConcurrentDictionary<string, FileDescriptor>(StringComparer.OrdinalIgnoreCase);

        private FileDescriptor CreateDescriptor(Guid organizationId, string path)
        {
            var entity = UploadSearch.Bind(organizationId, path, FileEntity.BindExpr);

            return new FileDescriptor(organizationId, path, entity);
        }

        private FileDescriptor CreateDescriptor(string path)
        {
            var entity = UploadSearch.Bind(FileEntity.BindExpr, x => x.NavigateUrl == path).SingleOrDefault();

            return new FileDescriptor(path, entity);
        }

        public FileDescriptor GetDescriptor(Guid organizationId, string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var descriptors = _organizations.GetOrAdd(organizationId, CreateDescriptorsStorage);
            var descriptor = descriptors.AddOrUpdate(
                path,
                key => CreateDescriptor(organizationId, key),
                (key, value) => value.IsExpired ? CreateDescriptor(organizationId, key) : value);

            return descriptor.IsNull ? null : descriptor;
        }

        public FileDescriptor GetDescriptor(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var descriptor = _paths.AddOrUpdate(
                path,
                CreateDescriptor,
                (key, value) => value.IsExpired ? CreateDescriptor(key) : value);

            return descriptor.IsNull ? null : descriptor;
        }

        private void OnDescriptorRemoved(FileModel model)
        {
            _organizations.GetOrAdd(model.OrganizationIdentifier, CreateDescriptorsStorage).AddOrUpdate(
                model.Path,
                key => FileDescriptor.GetNull(),
                (key, value) => FileDescriptor.GetNull());
            _paths.AddOrUpdate(
                model.Path,
                key => FileDescriptor.GetNull(),
                (key, value) => FileDescriptor.GetNull()
             );
        }

        private void OnDescriptorUpdated(FileModel model)
        {
            _organizations.GetOrAdd(model.OrganizationIdentifier, CreateDescriptorsStorage).TryRemove(model.Path, out FileDescriptor descriptor);
        }

        #endregion

        #region File Management

        public FileModel OpenModel(Guid organizationId, string path)
        {
            using (var storage = CreateFileStorage())
                return storage.Open(organizationId, path);
        }

        public Stream Read(Guid organizationId, string path)
        {
            var descriptor = GetDescriptor(organizationId, path);

            return Read(descriptor);
        }

        public Stream Read(FileDescriptor descriptor)
        {
            if (descriptor != null && !descriptor.IsNull)
            {
                var filePath = Path.Combine(StoragePath, descriptor.UploadId + descriptor.Type);
                var isException = true;

                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        if (System.IO.File.Exists(filePath))
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
                    descriptor.Expire();
            }

            return Stream.Null;
        }

        public FileModel Save(Guid organizationId, string path, string data) =>
            Save(organizationId, path, data, UTF8);

        public FileModel Save(Guid organizationId, string path, string data, Encoding encoding)
        {
            return Save(organizationId, path, stream =>
            {
                var writer = new StreamWriter(stream, encoding);
                writer.Write(data);
                writer.Flush();
            });
        }

        public FileModel Save(Guid organizationId, string path, byte[] data)
        {
            return Save(organizationId, path, stream =>
            {
                var writer = new BinaryWriter(stream);
                writer.Write(data);
                writer.Flush();
            });
        }

        public FileModel Save(Guid organizationId, string path, Stream stream, Guid? userId = null, bool isCheckFileSizeLimits = true) =>
            Update(organizationId, path, model => model.Write(stream, isCheckFileSizeLimits), userId);

        public FileModel Save(Guid organizationId, string path, Action<Stream> write) =>
            Update(organizationId, path, model => model.Write(write));

        public FileModel Update(Guid organizationId, string path, Action<FileModel> action, Guid? userId = null) => Update((FileStorage storage) =>
        {
            var model = storage.Open(organizationId, path);

            if (model.IsNew)
            {
                model.OrganizationIdentifier = organizationId;
                model.Path = path;
                model.UserIdentifier = userId;
            }

            action(model);

            return model;
        });

        public FileModel Update(Guid uploadId, Action<FileModel> action) => Update((FileStorage storage) =>
        {
            var model = storage.Open(uploadId);

            action(model);

            return model;
        });

        private FileModel Update(Func<FileStorage, FileModel> update)
        {
            using (var storage = CreateFileStorage())
            {
                var model = update(storage);

                model.Save();

                storage.Commit();

                return model;
            }
        }

        public FileModel Move(Guid organizationId, string sourcePath, string destinationPath)
        {
            using (var storage = CreateFileStorage())
            {
                var model = storage.Open(organizationId, sourcePath);
                if (model.IsNew)
                    return null;

                model.Path = destinationPath;
                model.Save();

                storage.Commit();

                return model;
            }
        }

        public FileModel Copy(Guid organizationId, string sourcePath, string destinationPath)
        {
            using (var storage = CreateFileStorage())
            {
                var sourceModel = storage.Open(organizationId, sourcePath);
                if (sourceModel.IsNew)
                    return null;

                using (var sourceStream = sourceModel.Read())
                {
                    if (sourceStream == Stream.Null)
                        return null;

                    var destinationModel = storage.Open(organizationId, destinationPath);

                    if (destinationModel.IsNew)
                    {
                        destinationModel.OrganizationIdentifier = organizationId;
                        destinationModel.Path = destinationPath;
                    }

                    destinationModel.Write(sourceStream);
                    destinationModel.Save();

                    storage.Commit();

                    return destinationModel;
                }
            }
        }

        public void Delete(Guid organizationId, string path)
        {
            if (path == null)
                return;

            using (var storage = CreateFileStorage())
            {
                var file = storage.Open(organizationId, path);
                if (file.IsNew)
                    return;

                file.Delete();

                storage.Commit();
            }
        }

        public void Delete(Guid uploadId)
        {
            using (var storage = CreateFileStorage())
            {
                var file = storage.Open(uploadId);
                if (file.IsNew)
                    return;

                file.Delete();

                storage.Commit();
            }
        }

        #endregion

        #region Helper methods

        private FileStorage CreateFileStorage()
        {
            var storage = new FileStorage(this);
            storage.EntityUpdated += OnDescriptorUpdated;
            storage.EntityRemoved += OnDescriptorRemoved;
            return storage;
        }

        #endregion
    }
}
