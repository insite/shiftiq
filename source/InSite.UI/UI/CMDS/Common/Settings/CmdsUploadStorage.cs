using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using Path = System.IO.Path;

namespace InSite.Cmds.Infrastructure
{
    public interface ICmdsUploadModel
    {
        Guid UploadID { get; }
        Guid ContainerID { get; set; }
        string ContainerType { get; set; }
        string Name { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        Guid Uploader { get; }
        DateTimeOffset Uploaded { get; }
        int ContentSize { get; }

        bool IsNew { get; }

        Stream Read();
        void Write(Stream stream);
        void Write(Action<Stream> write);
        void Save();
        void Delete();
    }

    public sealed class CmdsUploadStorage : IDisposable
    {
        #region Constants

        private static readonly IReadOnlyCollection<char> InvalidCharacters = new HashSet<char>(new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' });

        #endregion

        #region Enums

        private enum TransactionStep
        {
            Step1, Step2, Step3, Step4
        }

        #endregion

        #region Classes

        private class UploadModel : ICmdsUploadModel
        {
            #region Constants

            private static class TempFileType
            {
                public const string New = "new";
                public const string Removed = "delete";
                public const string Locked = "lock";
            }

            #endregion

            #region Properties

            public Guid UploadID => _uploadId ?? Guid.Empty;

            public Guid ContainerID { get; set; }

            public string Name { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public int ContentSize { get; private set; }

            public Guid Uploader { get; private set; }

            public DateTimeOffset Uploaded { get; private set; }

            public string ContainerType { get; set; }

            public bool IsNew => !_uploadId.HasValue;

            public FileStorageActionType Action => _action;

            private bool IsPathChanged => _originalKey.ContainerId != ContainerID || !StringComparer.OrdinalIgnoreCase.Equals(_originalKey.Name, Name);

            public CmdsUploadKey OriginalKey => _originalKey;

            #endregion

            #region Fields

            private Guid? _uploadId;
            private CmdsUploadKey _originalKey;
            private CmdsUploadStorage _storage;
            private FileStorageActionType _action;
            private TransactionStep _step;
            private Upload _entity;

            private Guid _tempId = UniqueIdentifier.Create();
            private bool _hasData;

            #endregion

            #region Construction

            public UploadModel(Guid guid, string name, string containerType, CmdsUploadStorage storage)
            {
                ContainerID = guid;
                Name = name;
                ContainerType = containerType;

                _storage = storage;
                _action = FileStorageActionType.None;
                _step = TransactionStep.Step1;
            }

            public UploadModel(Upload entity, CmdsUploadStorage storage)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                ContainerID = entity.ContainerIdentifier;
                ContainerType = entity.ContainerType;
                Name = entity.Name;
                Title = entity.Title;
                Description = entity.Description;
                ContentSize = entity.ContentSize ?? -1;
                Uploader = entity.Uploader;
                Uploaded = entity.Uploaded;

                _uploadId = entity.UploadIdentifier;
                _originalKey = new CmdsUploadKey(entity.ContainerIdentifier, entity.Name);
                _storage = storage;
                _action = FileStorageActionType.None;
                _step = TransactionStep.Step1;
            }

            #endregion

            #region Methods

            public Stream Read()
            {
                var path = _hasData
                    ? GetTempFilePath(TempFileType.New)
                    : GetEntityFilePath();

                if (string.IsNullOrEmpty(path))
                    return Stream.Null;

                return TryExecuteFunc(() =>
                {
                    return File.Exists(path)
                        ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                        : Stream.Null;
                });
            }

            public void Write(Stream inputStream)
            {
                ValidateWriteAccess();

                var path = GetTempFilePath(TempFileType.New);

                var fileType = Path.GetExtension(Name);
                var fileImageType = FileExtension.GetImageType(fileType);
                if (fileImageType == ImageType.Null || !WriteImage(path, inputStream, fileImageType))
                    CreateFile(path, outputStream => inputStream.CopyTo(outputStream));
            }

            public void Write(Action<Stream> write)
            {
                ValidateWriteAccess();

                var path = GetTempFilePath(TempFileType.New);

                CreateFile(path, write);
            }

            public void Save()
            {
                if (_action == FileStorageActionType.Delete)
                    throw ApplicationError.Create("Invalid state: {0}.", _action);

                _action = FileStorageActionType.Update;

                _storage.AddAction(this);
            }

            public void Delete()
            {
                _action = FileStorageActionType.Delete;

                _storage.AddAction(this);
            }

            public void Step1() // Validation
            {
                if (_step != TransactionStep.Step1)
                    throw ApplicationError.Create("Unexpected transaction state: {0}", _step);

                if (_action == FileStorageActionType.Update)
                {
                    if (ContainerID == Guid.Empty)
                        throw ApplicationError.Create("Invalid Container ID");

                    if (string.IsNullOrWhiteSpace(ContainerType))
                        throw ApplicationError.Create("Invalid Container Type");

                    if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(Path.GetExtension(Name)) || Name.Length > 500 || Name.Any(ch => InvalidCharacters.Contains(ch)))
                        throw ApplicationError.Create("Invalid name: {0}", Name);

                    if (string.IsNullOrWhiteSpace(Title) || Title.Length > 256)
                        Title = Name;

                    if (Title.Length > 256)
                        Title = Title.Substring(0, 250);

                    if (Description != null)
                        Description = Description.Trim();

                    if (ContentSize == 0)
                        throw ApplicationError.Create("File is empty.");

                    if (!IsNew && IsPathChanged)
                    {
                        var existModel = _storage.Open(ContainerID, Name, ContainerType);
                        if (!existModel.IsNew)
                            existModel.Delete();
                    }
                }
                else if (_action == FileStorageActionType.Delete)
                {

                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);

                _step = TransactionStep.Step2;
            }

            public void Step2(List<Upload> delete) // Prepare
            {
                if (_step != TransactionStep.Step2)
                    throw ApplicationError.Create("Unexpected transaction state: {0}", _step);

                if (_action == FileStorageActionType.Update)
                {
                    var modelFilePath = GetModelFilePath();

                    if (!IsNew)
                    {
                        var entityFilePath = GetEntityFilePath();
                        if (File.Exists(entityFilePath))
                        {
                            var lockPath = GetTempFilePath(TempFileType.Locked);
                            TryExecuteAction(() => File.Move(entityFilePath, lockPath));
                        }
                    }

                    if (_hasData)
                    {
                        var newPath = GetTempFilePath(TempFileType.New);

                        TryExecuteAction(() =>
                        {
                            EnsureDirectoryExists(modelFilePath);

                            if (File.Exists(modelFilePath))
                                File.Delete(modelFilePath);

                            File.Move(newPath, modelFilePath);
                        });
                    }
                }
                else if (_action == FileStorageActionType.Delete)
                {
                    if (!IsNew)
                    {
                        var entityFilePath = GetEntityFilePath();
                        var removePath = GetTempFilePath(TempFileType.Removed);

                        if (File.Exists(entityFilePath))
                            TryExecuteAction(() => File.Move(entityFilePath, removePath));

                        delete.Add(new Upload { UploadIdentifier = _uploadId.Value });
                    }
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);

                _step = TransactionStep.Step3;
            }

            public void Step3(List<Upload> insert, List<Upload> update) // Commit
            {
                if (_step != TransactionStep.Step3)
                    throw ApplicationError.Create("Unexpected transaction state: {0}", _step);

                if (_action == FileStorageActionType.Update)
                {
                    _entity = !IsNew
                        ? UploadSearch.BindFirst(x => x, x => x.ContainerIdentifier == _originalKey.ContainerId && x.Name == _originalKey.Name)
                        : null;

                    if (_entity == null)
                    {
                        insert.Add(_entity = new Upload
                        {
                            UploadPrivacyScope = Common.Web.FileAccessScopes.Platform,
                            UploadIdentifier = UniqueIdentifier.Create(),
                            UploadType = UploadType.CmdsFile,
                            OrganizationIdentifier = ContainerType == UploadContainerType.Oganization
                                ? ContainerID
                                : CurrentSessionState.Identity.Organization.Identifier,

                            Uploader = CurrentSessionState.Identity.User.UserIdentifier,
                            Uploaded = DateTimeOffset.UtcNow
                        });
                    }
                    else
                    {
                        update.Add(_entity);
                    }

                    _entity.ContainerIdentifier = ContainerID;
                    _entity.Name = Name;
                    _entity.Title = Title;
                    _entity.Description = Description;
                    _entity.ContentSize = ContentSize;
                    _entity.ContainerType = ContainerType;

                    if (!_hasData)
                    {
                        var lockPath = GetTempFilePath(TempFileType.Locked);
                        var modelFilePath = GetModelFilePath();

                        if (File.Exists(lockPath))
                            TryExecuteAction(() =>
                            {
                                EnsureDirectoryExists(modelFilePath);

                                File.Move(lockPath, modelFilePath);
                            });
                    }
                    else if (!IsNew)
                    {
                        var lockPath = GetTempFilePath(TempFileType.Locked);
                        if (File.Exists(lockPath))
                            TryExecuteAction(() => File.Delete(lockPath));
                    }
                }
                else if (_action == FileStorageActionType.Delete)
                {
                    var removePath = GetTempFilePath(TempFileType.Removed);
                    if (File.Exists(removePath))
                        TryExecuteAction(() => File.Delete(removePath));
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);

                _step = TransactionStep.Step4;
            }

            public void Step4()
            {
                if (_step != TransactionStep.Step4)
                    throw ApplicationError.Create("Unexpected transaction state: {0}", _step);

                if (_action == FileStorageActionType.Update)
                {
                    Uploader = _entity.Uploader;
                    Uploaded = _entity.Uploaded;

                    _uploadId = _entity.UploadIdentifier;
                    _originalKey = new CmdsUploadKey(_entity.ContainerIdentifier, _entity.Name);
                    _action = FileStorageActionType.None;
                    _step = TransactionStep.Step1;
                    _hasData = false;
                    _entity = null;
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);
            }

            public void Rollback()
            {
                if (_step == TransactionStep.Step4 || _action == FileStorageActionType.None)
                    return;

                if (_action == FileStorageActionType.Update)
                {
                    var modelFilePath = GetModelFilePath();
                    var entityFilePath = GetEntityFilePath();

                    if (!IsNew)
                    {
                        var lockPath = GetTempFilePath(TempFileType.Locked);
                        if (File.Exists(lockPath))
                        {
                            if (File.Exists(modelFilePath))
                                TryExecuteAction(() => File.Delete(modelFilePath));

                            TryExecuteAction(() => File.Move(lockPath, entityFilePath));
                        }
                    }
                    else
                    {
                        var newPath = GetTempFilePath(TempFileType.New);
                        if (File.Exists(newPath))
                            TryExecuteAction(() => File.Delete(newPath));

                        if (File.Exists(modelFilePath))
                            TryExecuteAction(() => File.Delete(modelFilePath));
                    }
                }
                else if (_action == FileStorageActionType.Delete)
                {
                    if (!IsNew)
                    {
                        var entityFilePath = GetEntityFilePath();
                        var removePath = GetTempFilePath(TempFileType.Removed);

                        if (File.Exists(removePath))
                            TryExecuteAction(() => File.Move(removePath, entityFilePath));
                    }
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);
            }

            public void Dispose()
            {
                Rollback();

                var newPath = GetTempFilePath(TempFileType.New);
                if (File.Exists(newPath))
                    TryExecuteAction(() => File.Delete(newPath));

                _storage = null;
                _hasData = false;
            }

            #endregion

            #region Helper methods (file system)

            private string GetEntityFilePath() =>
                IsNew ? null : Path.Combine(_storage._provider.StoragePath, _originalKey.ContainerId.ToString(), _originalKey.Name);

            private string GetModelFilePath() =>
                Path.Combine(_storage._provider.StoragePath, ContainerID.ToString(), Name);

            private string GetTempFilePath(string ext) =>
                Path.Combine(_storage._provider.TempStoragePath, _storage.ID + "_" + _tempId + "." + ext);

            private static void EnsureDirectoryExists(string filepath)
            {
                var dirPath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }

            private void CreateFile(string path, Action<Stream> write)
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    write(fileStream);

                    ContentSize = (int)fileStream.Length;
                }

                if (ContentSize == 0)
                    throw ApplicationError.Create("File is empty.");

                _hasData = true;
            }

            private bool WriteImage(string path, Stream inputStream, ImageType fileImageType)
            {
                try
                {
                    var imageInfo = ImageHelper.ReadInfo(inputStream);
                    var imageType = imageInfo.ImageType;
                    var isTypeChanged = false;

                    if (imageType != ImageType.Png && imageType != ImageType.Gif && imageType != ImageType.Jpeg)
                    {
                        imageType = imageInfo.ColorSpace == ColorSpace.Gray
                            ? ImageType.Png
                            : ImageType.Jpeg;
                        isTypeChanged = true;
                    }
                    else if (fileImageType != imageType)
                    {
                        isTypeChanged = true;
                    }

                    if (isTypeChanged)
                        Name = Path.GetFileNameWithoutExtension(Name) + FileExtension.GetImageExtension(imageType);

                    var messages = new List<string>();

                    CreateFile(path, outputStream => ImageHelper.AdjustImage(inputStream, outputStream, imageType, false, messages));

                    return true;
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);

                    return false;
                }
            }

            #endregion

            #region Helper methods

            private void ValidateWriteAccess()
            {
                if (_storage == null || _storage._isDisposed)
                    throw ApplicationError.Create("Context is null.");
            }

            #endregion
        }

        #endregion

        #region Delegates

        public delegate void CommitEventHandler(ICmdsUploadModel item);

        #endregion

        #region Events

        public event CommitEventHandler EntityRemoved;
        private void OnEntityRemoved(ICmdsUploadModel item) => EntityRemoved?.Invoke(item);

        public event CommitEventHandler EntityUpdated;
        private void OnEntityUpdated(ICmdsUploadModel item) => EntityUpdated?.Invoke(item);

        #endregion

        #region Properties

        public Guid ID => _id;

        #endregion

        #region Fields

        private Guid _id;
        private CmdsUploadProvider _provider;
        private Dictionary<CmdsUploadKey, UploadModel> _files;
        private List<UploadModel> _actions;
        private bool _isDisposed;

        #endregion

        #region Construction

        public CmdsUploadStorage(CmdsUploadProvider provider)
        {
            _id = UniqueIdentifier.Create();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _files = new Dictionary<CmdsUploadKey, UploadModel>();
            _actions = new List<UploadModel>();
            _isDisposed = false;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed)
                return;

            foreach (var info in _files)
                info.Value.Dispose();

            _id = Guid.Empty;
            _provider = null;
            _files = null;
            _actions = null;

            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        public ICmdsUploadModel Open(Guid containerId, string name, string containerType)
        {
            var key = new CmdsUploadKey(containerId, name);
            if (_files.ContainsKey(key))
                return _files[key];

            var entity = UploadSearch.Select(containerId, name);

            var model = entity == null
                ? new UploadModel(containerId, name, containerType, this)
                : new UploadModel(entity, this);

            _files.Add(key, model);

            return model;
        }

        public ICmdsUploadModel Open(Guid id)
        {
            var entity = UploadSearch.Select(id);
            if (entity == null)
                return null;

            var key = new CmdsUploadKey(entity.ContainerIdentifier, entity.Name);
            if (_files.ContainsKey(key))
                return _files[key];

            var model = new UploadModel(entity, this);

            _files.Add(key, model);

            return model;
        }

        private void AddAction(UploadModel item)
        {
            _actions.Remove(item);
            _actions.Add(item);
        }

        internal void Commit()
        {
            var keys = new List<CmdsUploadKey>();

            foreach (var action in _actions)
            {
                if (action.Action == FileStorageActionType.None)
                    continue;

                if (!action.IsNew)
                    keys.Add(action.OriginalKey);

                var currentKey = new CmdsUploadKey(action.ContainerID, action.Name);
                if (action.IsNew || !action.OriginalKey.Equals(currentKey))
                    keys.Add(currentKey);
            }

            CmdsUploadQueue.Execute(ID, keys, () => CommitInternal());
        }

        private void CommitInternal()
        {
            try
            {
                for (var i = 0; i < _actions.Count; i++)
                    _actions[i].Step1();

                var insert = new List<Upload>();
                var update = new List<Upload>();
                var delete = new List<Upload>();

                for (var i = 0; i < _actions.Count; i++)
                    _actions[i].Step2(delete);

                for (var i = 0; i < _actions.Count; i++)
                    _actions[i].Step3(insert, update);

                UploadStore.Commit(insert, update, delete);
            }
            catch
            {
                Rollback();

                throw;
            }

            foreach (var action in _actions)
            {
                if (action.Action == FileStorageActionType.Update)
                {
                    action.Step4();
                    OnEntityUpdated(action);
                }
                else if (action.Action == FileStorageActionType.Delete)
                {
                    if (!action.IsNew)
                    {
                        _files.Remove(action.OriginalKey);

                        action.Dispose();

                        OnEntityRemoved(action);
                    }
                }
            }
        }

        internal void Rollback()
        {
            for (var i = _actions.Count - 1; i >= 0; i--)
                _actions[i].Rollback();

            _actions.Clear();
        }

        #endregion

        #region Helper methods

        private static void TryExecuteAction(System.Action action)
        {
            Exception lastException = null;

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    Thread.Sleep(100);
                }
            }

            throw lastException ?? new Exception("Unable execute");
        }

        private static T TryExecuteFunc<T>(Func<T> func)
        {
            Exception lastException = null;

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    Thread.Sleep(100);
                }
            }

            throw lastException ?? new Exception("Unable execute");
        }

        #endregion
    }
}