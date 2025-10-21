using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.UI;

using InSite.Application.Files.Read;


using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.File;

using IFFmpegProbeResult = Shift.Common.FFmpeg.IProbeResult;

namespace InSite.Common.Web.UI
{
    public abstract class InputMedia : BaseControl, IPostBackDataHandler
    {
        #region Events

        public event EventHandler MediaCaptured;
        protected void OnMediaCaptured() =>
            MediaCaptured?.Invoke(this, EventArgs.Empty);

        public event StringValueHandler MediaCaptureFailed;
        protected void OnMediaCaptureFailed(string message) =>
            MediaCaptureFailed?.Invoke(this, new StringValueArgs(message));

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class BaseClientData
        {
            [JsonProperty(PropertyName = "file", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public UploadFileInfo File { get; set; }

            public bool IsMediaCaptured { get; set; }
        }

        protected interface IMediaFile
        {
            string Name { get; }
            int Size { get; }

            string ValidationError { get; set; }
            bool IsValid { get; }

            void Open(Action<Stream> action);
            FileStorageModel Save(Guid objectIdentifier, FileObjectType objectType, string fileName = null, bool checkFileValid = true);
            void Delete();
        }

        [Serializable]
        protected abstract class BaseMediaFile : IMediaFile
        {
            public abstract string Name { get; }
            public abstract int Size { get; }

            public string ValidationError { get; set; }
            public bool IsValid => ValidationError == null;

            private bool _isSaved;

            [NonSerialized]
            private InputMedia _control;

            public BaseMediaFile(InputMedia control)
            {
                _control = control;
            }

            public abstract void Open(Action<Stream> action);

            public FileStorageModel Save(Guid objectIdentifier, FileObjectType objectType, string fileName = null, bool checkFileValid = true)
            {
                if (checkFileValid && !IsValid)
                    throw ApplicationError.Create("The file is not valid.");

                if (_isSaved)
                    throw ApplicationError.Create("The file has already been saved.");

                var model = SaveInternal(objectIdentifier, objectType, fileName);

                _isSaved = true;

                return model;
            }

            public void Delete()
            {
                if (_isSaved)
                    throw ApplicationError.Create("The file has already been saved.");

                DeleteInternal();

                if (_control != null)
                    _control._mediaFile = null;
            }

            protected abstract FileStorageModel SaveInternal(Guid objId, FileObjectType objType, string fileName);

            public abstract void DeleteInternal();
        }

        protected class StreamMediaFile : BaseMediaFile
        {
            public override string Name => _fileName;

            public override int Size => (int)_stream.Length;

            private string _fileName;
            private Stream _stream;

            public StreamMediaFile(InputMedia control, string fileName, Stream stream) : base(control)
            {
                _fileName = fileName.NullIfEmpty() ?? throw new ArgumentNullException(nameof(fileName));
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            }

            public override void Open(Action<Stream> action)
            {
                _stream.Seek(0, SeekOrigin.Begin);

                action(_stream);
            }

            protected override FileStorageModel SaveInternal(Guid objId, FileObjectType objType, string fileName = null)
            {
                var identity = CurrentSessionState.Identity;
                var orgId = identity.Organization.Identifier;
                var userId = identity.User?.Identifier ?? Shift.Constant.UserIdentifiers.Someone;
                var name = fileName.IfNullOrEmpty(_fileName);

                _stream.Seek(0, SeekOrigin.Begin);

                return ServiceLocator.StorageService.Create(
                    _stream,
                    name,
                    orgId,
                    userId,
                    objId,
                    objType,
                    new FileProperties { DocumentName = name },
                    null
                );
            }

            public override void DeleteInternal()
            {

            }
        }

        protected class HttpMediaFile : StreamMediaFile
        {
            public HttpMediaFile(InputMedia control, HttpPostedFile file)
                : base(control, file.FileName, file.InputStream)
            {
            }
        }

        [Serializable]
        protected class ApiMediaFile : BaseMediaFile
        {
            public override string Name => _file.FileName;

            public override int Size => _file.FileSize;

            private FileStorageModel _file;

            public ApiMediaFile(InputMedia control, FileStorageModel file) : base(control)
            {
                _file = file ?? throw new ArgumentNullException(nameof(file));
            }

            public override void Open(Action<Stream> action)
            {
                using (var stream = ServiceLocator.StorageService.GetFileStream(_file.FileIdentifier).Item2)
                    action(stream);
            }

            protected override FileStorageModel SaveInternal(Guid objId, FileObjectType objType, string fileName)
            {
                return FileUploadV2.SaveFile(_file, objId, objType, fileName);
            }

            public override void DeleteInternal()
            {
                ServiceLocator.StorageService.Delete(_file.FileIdentifier);
            }
        }

        #endregion

        #region Properties

        public bool Enabled
        {
            get => (bool)(ViewState[nameof(Enabled)] ?? true);
            set => ViewState[nameof(Enabled)] = value;
        }

        public virtual bool ReadOnly
        {
            get { return (bool)(ViewState[nameof(ReadOnly)] ?? false); }
            set { ViewState[nameof(ReadOnly)] = value; }
        }

        public bool AllowPause
        {
            get => (bool)(ViewState[nameof(AllowPause)] ?? true);
            set => ViewState[nameof(AllowPause)] = value;
        }

        public InputMediaUpload UploadMode
        {
            get => (InputMediaUpload)(ViewState[nameof(UploadMode)] ?? InputMediaUpload.PostBack);
            set => ViewState[nameof(UploadMode)] = value;
        }

        public bool AutoUpload
        {
            get => (bool)(ViewState[nameof(AutoUpload)] ?? true);
            set => ViewState[nameof(AutoUpload)] = value;
        }

        public bool AutoPostBack
        {
            get => (bool)(ViewState[nameof(AutoPostBack)] ?? false);
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? true);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public string ValidationGroup
        {
            get => (string)(ViewState[nameof(ValidationGroup)] ?? string.Empty);
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        public bool Hidden
        {
            get => (bool)(ViewState[nameof(Hidden)] ?? false);
            set => ViewState[nameof(Hidden)] = value;
        }

        protected string SubmitScript => _submitScript;

        #endregion

        #region Fields

        private BaseClientData _clientData;
        private BaseMediaFile _mediaFile;
        private string _submitScript;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            {
                var postBackOptions = new PostBackOptions(this, string.Empty)
                {
                    AutoPostBack = AutoPostBack
                };

                if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
                {
                    postBackOptions.PerformValidation = true;
                    postBackOptions.ValidationGroup = ValidationGroup;
                }

                _submitScript = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler

        protected virtual BaseClientData DeserializeClientData(string data)
        {
            return JsonConvert.DeserializeObject<BaseClientData>(data);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            if (!Visible || ReadOnly)
                return false;

            _clientData = DeserializeClientData(postCollection[postDataKey]);

            if (UploadMode == InputMediaUpload.PostBack)
            {
                var httpFile = Context.Request.Files[UniqueID + "$file"];
                if (_clientData.IsMediaCaptured = httpFile != null)
                    _mediaFile = new HttpMediaFile(this, httpFile);
            }
            else if (UploadMode == InputMediaUpload.API)
            {
                var apiFile = _clientData.File != null
                    ? ServiceLocator.StorageService.GetFile(_clientData.File.FileIdentifier)
                    : null;

                if (_clientData.IsMediaCaptured = apiFile != null)
                    _mediaFile = new ApiMediaFile(this, apiFile);
            }

            return LoadPostData(_clientData) || _clientData.IsMediaCaptured;
        }

        protected virtual bool LoadPostData(BaseClientData clientData)
        {
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (AutoPostBack && !Page.IsPostBackEventControlRegistered)
            {
                Page.AutoPostBackControl = this;

                if (CausesValidation)
                    Page.Validate(ValidationGroup);
            }

            RaisePostDataChangedEvent(_clientData);

            if (_clientData.IsMediaCaptured)
            {
                SetMediaFile(_mediaFile, _clientData);

                if (_mediaFile.IsValid)
                    OnMediaCaptured();
                else
                    OnMediaCaptureFailed(_mediaFile.ValidationError);
            }

            _clientData = null;
        }

        protected virtual void RaisePostDataChangedEvent(BaseClientData clientData)
        {

        }

        #endregion

        #region Validation

        protected abstract void SetMediaFile(BaseMediaFile file, BaseClientData data);

        protected static IFFmpegProbeResult ProbeMedia(IMediaFile file)
        {
            IFFmpegProbeResult probeData = null;

            file.Open(stream =>
            {
                try
                {
                    probeData = ServiceLocator.FFmpeg.Probe(stream, FFmpeg.ProbeType.Streams | FFmpeg.ProbeType.Packets);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);
                }
            });

            if (probeData == null)
            {
                file.ValidationError = "Unable to read captured media data";
                return null;
            }

            if (probeData.Streams == null || probeData.Streams.Length == 0)
            {
                file.ValidationError = "No stream found";
                return null;
            }

            if (probeData.Packets == null || probeData.Packets.Length == 0)
            {
                file.ValidationError = "No packets found";
                return null;
            }

            return probeData;
        }

        #endregion
    }
}