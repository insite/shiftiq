using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

using Humanizer;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Common.Web
{
    public static class FileAccessScopes
    {
        public static string Contact => "Contact";
        public static string Platform => "Platform";
        public static string Public => "Public";
        public static string Organization => "Tenant";
    }

    public sealed class FileStorage : IDisposable
    {
        #region Enums

        private enum TransactionStep
        {
            Step1, Step2, Step3, Step4
        }

        #endregion

        #region Classes

        public class MaxFileSizeExceededException : ApplicationError
        {
            public string FileName { get; private set; }
            public string FileSize { get; private set; }
            public string FileSizeLimit { get; private set; }
            public string FileType { get; private set; }

            public override string Message => GetMessage(FileType, FileSizeLimit);

            public MaxFileSizeExceededException(string fileType, long fileSizeLimit)
            {
                FileType = fileType;
                FileSizeLimit = HumanizeFileSize(fileSizeLimit);
            }

            public MaxFileSizeExceededException(string fileType, string fileName, long fileSize, long fileSizeLimit)
            {
                FileName = fileName;
                FileSize = HumanizeFileSize(fileSize);
                FileSizeLimit = HumanizeFileSize(fileSizeLimit);
                FileType = fileType;
            }

            public static MaxFileSizeExceededException Create(string fileType, string fileName, long fileSize, long fileSizeLimit)
            {
                return new MaxFileSizeExceededException(fileType, fileName, fileSize, fileSizeLimit);
            }

            public static string GetMessage(string fileType, long fileSizeLimit) =>
                GetMessage(fileType, HumanizeFileSize(fileSizeLimit));

            public static string GetMessage(string fileType, string fileSizeLimit) =>
                $"Your account is configured with a file size limit of {fileSizeLimit} per {fileType}. " +
                $"Please decrease the size of your file before uploading it, or contact your administrator to upgrade the settings for your account.";

            private static string HumanizeFileSize(long value) => value.Bytes().Humanize("MB");
        }

        private class Changes
        {
            public List<Upload> Insert { get; private set; } = new List<Upload>();
            public List<Upload> Update { get; private set; } = new List<Upload>();
            public List<Upload> Delete { get; private set; } = new List<Upload>();
        }

        private class TransactionInfo : IFileStorageItem
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

            public Guid FileId { get; private set; }
            public Guid OrganizationIdentifier => _organizationId;

            public string Path => _entity?.NavigateUrl;

            public int ContentSize => _contentSize ?? _entity?.ContentSize ?? 0;
            public string DataFingerprint => _dataFingerprint ?? _entity?.ContentFingerprint;

            public Guid? Uploader => _entity?.Uploader;
            public DateTimeOffset Uploaded => _entity?.Uploaded ?? DateTimeOffset.MinValue;

            public FileModel Model => _model;

            public bool IsNew => _isNew;

            public FileStorageActionType Action => _action;

            #endregion

            #region Fields

            private Upload _entity;
            private Guid _organizationId;
            private FileModel _model;
            private FileStorage _storage;
            private FileStorageActionType _action;
            private TransactionStep _step;

            private bool _isNew;
            private bool _hasData;
            private int? _contentSize;
            private string _dataFingerprint;

            #endregion

            #region Construction

            public TransactionInfo(FileStorage storage, Upload entity, Guid organizationId)
            {
                FileId = entity?.UploadIdentifier ?? UniqueIdentifier.Create();

                _storage = storage;
                _entity = entity;
                _organizationId = organizationId;
                _model = new FileModel(this);
                _action = FileStorageActionType.None;
                _step = TransactionStep.Step1;
                _isNew = entity == null;
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
                    return System.IO.File.Exists(path)
                        ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                        : Stream.Null;
                });
            }

            public void Write(Stream inputStream, bool isCheckFileSizeLimits)
            {
                ValidateWriteAccess();

                var path = GetTempFilePath(TempFileType.New);
                var fileImageType = FileExtension.GetImageType(_model.Type);
                var isImage = fileImageType != ImageType.Null;

                var uploadSettings = OrganizationSearch.Select(CookieTokenModule.Current.OrganizationCode)?.PlatformCustomization?.UploadSettings
                    ?? new Domain.Organizations.UploadSettings();

                if (isImage)
                {
                    var limit = uploadSettings.Images.MaximumFileSize;
                    if (isCheckFileSizeLimits && inputStream.Length > limit)
                        throw new MaxFileSizeExceededException("image", path, (int)inputStream.Length, limit);

                    isImage = WriteImage(path, inputStream, fileImageType, uploadSettings.Images.MaximumWidth, uploadSettings.Images.MaximumHeight);
                }
                else if (isCheckFileSizeLimits && inputStream.Length > uploadSettings.Documents.MaximumFileSize)
                {
                    var limit = uploadSettings.Documents.MaximumFileSize;
                    throw new MaxFileSizeExceededException("document", path, (int)inputStream.Length, limit);
                }

                if (!isImage)
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
                    if (_model.OrganizationIdentifier == Guid.Empty)
                        throw ApplicationError.Create("Organization is null.");

                    if (!FileModel.IsPathValid(_model.Path))
                        throw ApplicationError.Create("Invalid file path: {0}.", _model.Path);

                    if (IsNew && !_hasData)
                        throw ApplicationError.Create("File is empty: {0}.", _model.Path);

                    var existModel = _storage.Open(_model.OrganizationIdentifier, _model.Path);
                    if (!existModel.IsNew && existModel.Guid != FileId)
                        existModel.Delete();
                }
                else if (_action == FileStorageActionType.Delete)
                {

                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);

                _step = TransactionStep.Step2;
            }

            public void Step2(Changes changes) // Prepare
            {
                if (_step != TransactionStep.Step2)
                    throw ApplicationError.Create("Unexpected transaction state: {0}", _step);

                if (_action == FileStorageActionType.Update)
                {
                    var modelFilePath = GetModelFilePath();

                    if (!IsNew)
                    {
                        var entityFilePath = GetEntityFilePath();

                        if (_hasData || string.Equals(modelFilePath, entityFilePath, StringComparison.OrdinalIgnoreCase))
                        {
                            var lockPath = GetTempFilePath(TempFileType.Locked);
                            if (System.IO.File.Exists(entityFilePath))
                                TryExecuteAction(() => System.IO.File.Move(entityFilePath, lockPath));
                        }
                    }

                    if (_hasData)
                    {
                        var newPath = GetTempFilePath(TempFileType.New);
                        TryExecuteAction(() => System.IO.File.Move(newPath, modelFilePath));
                    }
                }
                else if (_action == FileStorageActionType.Delete)
                {
                    if (!IsNew)
                    {
                        var entityFilePath = GetEntityFilePath();
                        var removePath = GetTempFilePath(TempFileType.Removed);

                        if (System.IO.File.Exists(entityFilePath))
                            TryExecuteAction(() => System.IO.File.Move(entityFilePath, removePath));

                        changes.Delete.Add(_entity);
                    }
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);

                _step = TransactionStep.Step3;
            }

            public void Step3(Changes changes) // Commit
            {
                if (_step != TransactionStep.Step3)
                    throw ApplicationError.Create("Unexpected transaction state: {0}", _step);

                if (_action == FileStorageActionType.Update)
                {
                    if (!IsNew && UploadSearch.Exists(x => x.UploadIdentifier == FileId))
                    {
                        changes.Update.Add(_entity);
                    }
                    else
                    {
                        changes.Insert.Add(_entity = new Upload { UploadIdentifier = FileId });
                    }

                    _entity.UploadPrivacyScope = FileAccessScopes.Platform;
                    _entity.UploadType = UploadType.InSiteFile;
                    _entity.ContainerType = UploadContainerType.Oganization;
                    _entity.OrganizationIdentifier = _entity.ContainerIdentifier = _model.OrganizationIdentifier;
                    _entity.NavigateUrl = _model.Path;
                    _entity.Name = _entity.Title = _model.Name;
                    _entity.ContentType = _model.Type;

                    _entity.Uploader = CurrentSessionState.Identity != null ? CurrentSessionState.Identity.User.UserIdentifier : _model.UserIdentifier ?? Guid.Empty;
                    _entity.Uploaded = DateTimeOffset.UtcNow;

                    if (_hasData)
                    {
                        _entity.ContentSize = _contentSize.Value;
                        _entity.ContentFingerprint = _dataFingerprint;

                        if (!IsNew)
                        {
                            var lockPath = GetTempFilePath(TempFileType.Locked);
                            if (System.IO.File.Exists(lockPath))
                                TryExecuteAction(() => System.IO.File.Delete(lockPath));
                        }
                    }
                    else
                    {
                        var lockPath = GetTempFilePath(TempFileType.Locked);
                        var modelFilePath = GetModelFilePath();

                        if (System.IO.File.Exists(lockPath))
                            TryExecuteAction(() => System.IO.File.Move(lockPath, modelFilePath));
                    }

                    _isNew = false;
                }
                else if (_action == FileStorageActionType.Delete)
                {
                    var removePath = GetTempFilePath(TempFileType.Removed);
                    if (System.IO.File.Exists(removePath))
                        TryExecuteAction(() => System.IO.File.Delete(removePath));
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);

                _step = TransactionStep.Step4;
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
                        if (System.IO.File.Exists(lockPath))
                        {
                            if (System.IO.File.Exists(modelFilePath))
                                TryExecuteAction(() => System.IO.File.Delete(modelFilePath));

                            TryExecuteAction(() => System.IO.File.Move(lockPath, entityFilePath));
                        }
                    }
                    else
                    {
                        var newPath = GetTempFilePath(TempFileType.New);
                        if (System.IO.File.Exists(newPath))
                            TryExecuteAction(() => System.IO.File.Delete(newPath));

                        if (System.IO.File.Exists(modelFilePath))
                            TryExecuteAction(() => System.IO.File.Delete(modelFilePath));
                    }
                }
                else if (_action == FileStorageActionType.Delete)
                {
                    if (!IsNew)
                    {
                        var entityFilePath = GetEntityFilePath();
                        var removePath = GetTempFilePath(TempFileType.Removed);

                        if (System.IO.File.Exists(removePath))
                            TryExecuteAction(() => System.IO.File.Move(removePath, entityFilePath));
                    }
                }
                else
                    throw ApplicationError.Create("Unexpected action type: {0}", _action);
            }

            public void Dispose()
            {
                Rollback();

                var newPath = GetTempFilePath(TempFileType.New);
                if (System.IO.File.Exists(newPath))
                    TryExecuteAction(() => System.IO.File.Delete(newPath));

                _model = null;
                _storage = null;
                _contentSize = null;
                _dataFingerprint = null;
                _hasData = false;
            }

            #endregion

            #region Helper methods (file system)

            private string GetEntityFilePath() =>
                _entity == null ? null : System.IO.Path.Combine(_storage._provider.StoragePath, _entity.UploadIdentifier + _entity.ContentType);

            private string GetModelFilePath() =>
                System.IO.Path.Combine(_storage._provider.StoragePath, FileId + _model.Type);

            private string GetTempFilePath(string ext) =>
                System.IO.Path.Combine(_storage._provider.StoragePath, $"_{FileId}_{_storage.ID}.{ext}");

            private void CreateFile(string path, Action<Stream> write)
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var hashAlgorithm = MD5.Create())
                    {
                        using (var outputStream = new CryptoStream(fileStream, hashAlgorithm, CryptoStreamMode.Write))
                        {
                            write(outputStream);

                            outputStream.FlushFinalBlock();

                            _contentSize = (int)fileStream.Length;
                        }

                        _dataFingerprint = Convert.ToBase64String(hashAlgorithm.Hash);
                    }
                }

                if (_contentSize.Value == 0)
                    throw ApplicationError.Create("File is empty.");

                _hasData = true;
            }

            private bool WriteImage(string path, Stream inputStream, ImageType fileImageType, int maxImageWidth, int maxImageHeight)
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
                        _model.Type = FileExtension.GetImageExtension(imageType);

                    CreateFile(path, outputStream => ImageHelper.AdjustImage(inputStream, outputStream, imageType, false, _model.ActionMessages, maxImageWidth, maxImageHeight));

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            #endregion

            #region Helper methods

            private void ValidateWriteAccess()
            {
                if (_storage == null || _storage._isDisposed)
                    throw ApplicationError.Create("Context is null.");

                if (!FileModel.IsPathValid(_model.Path))
                    throw ApplicationError.Create("Invalid file path: {0}.", _model.Path);
            }

            #endregion
        }

        #endregion

        #region Delegates

        public delegate void CommitEventHandler(FileModel model);

        #endregion

        #region Events

        public event CommitEventHandler EntityRemoved;
        private void OnEntityRemoved(FileModel model) => EntityRemoved?.Invoke(model);

        public event CommitEventHandler EntityUpdated;
        private void OnEntityUpdated(FileModel model) => EntityUpdated?.Invoke(model);

        #endregion

        #region Properties

        public Guid ID => _id;

        #endregion

        #region Fields

        private Guid _id;
        private FileProvider _provider;
        private List<TransactionInfo> _files;
        private List<TransactionInfo> _actions;
        private bool _isDisposed;

        #endregion

        #region Construction

        public FileStorage(FileProvider provider)
        {
            _id = UniqueIdentifier.Create();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _files = new List<TransactionInfo>();
            _actions = new List<TransactionInfo>();
            _isDisposed = false;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed)
                return;

            foreach (var info in _files)
                info.Dispose();

            _id = Guid.Empty;
            _provider = null;
            _files = null;
            _actions = null;

            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        public FileModel Open(Guid organizationId, string path)
        {
            var info = _files.SingleOrDefault(
                x => x.OrganizationIdentifier == organizationId && string.Equals(x.Path, path, StringComparison.OrdinalIgnoreCase));

            if (info == null)
            {
                var entity = UploadSearch.SelectByPath(organizationId, path);

                info = CreateFileInfo(entity, organizationId);
            }

            return info.Model;
        }

        internal FileModel Open(Guid id)
        {
            var info = _files.SingleOrDefault(x => x.FileId == id);

            if (info == null)
            {
                var entity = UploadSearch.Select(id);

                info = CreateFileInfo(entity, entity.OrganizationIdentifier);
            }

            return info.Model;
        }

        private void AddAction(TransactionInfo item)
        {
            _actions.Remove(item);
            _actions.Add(item);
        }

        internal void Commit()
        {
            try
            {
                for (var i = 0; i < _actions.Count; i++)
                    _actions[i].Step1();

                var changes = new Changes();

                for (var i = 0; i < _actions.Count; i++)
                    _actions[i].Step2(changes);

                for (var i = 0; i < _actions.Count; i++)
                    _actions[i].Step3(changes);

                UploadStore.Commit(changes.Insert, changes.Update, changes.Delete);
            }
            catch
            {
                Rollback();

                throw;
            }

            foreach (var action in _actions)
            {
                if (action.Action == FileStorageActionType.Update)
                    OnEntityUpdated(new FileModel(action));
                else if (action.Action == FileStorageActionType.Delete)
                {
                    if (!action.IsNew)
                        OnEntityRemoved(new FileModel(action));
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

        #region Methods (file system)

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

        #region Helpers

        private TransactionInfo CreateFileInfo(Upload entity, Guid organizationId)
        {
            var info = new TransactionInfo(this, entity, organizationId);

            _files.Add(info);

            return info;
        }

        #endregion
    }
}
