using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Common.File;

using IoFile = System.IO.File;

namespace InSite.Common.Web.UI
{
    [ValidationProperty("FilePath")]
    public class FileUploadV1 : Control, IPostBackDataHandler, IPostBackEventHandler
    {
        #region Classes

        private class ClientSideStringSettings
        {
            public string FileNameLengthExceeded { get; set; }
        }

        private class Settings
        {
            public string SelectButtonId { get; set; }
            public string SelectFilesId { get; set; }
            public string SelectedFileNamesId { get; set; }
            public string UploadedFilesId { get; set; }
            public string UploadProgressId { get; set; }
            public string ContainerType { get; set; }
            public string ContainerIdentifier { get; set; }
            public int? MaxFileSize { get; set; }
            public int MaxFileNameLength { get; set; }
            public string AllowedExtensionsRegex { get; set; }
            public string OnClientFileUploaded { get; set; }
            public string UniqueId { get; set; }
            public string SessionIdentifier { get; set; }

            public ClientSideStringSettings Strings { get; set; }
        }

        #endregion

        #region Events and properties

        public event EventHandler FileUploaded;
        private void OnFileUploaded()
        {
            if (FileUploaded == null)
                return;

            InputText = Metadata?.FileName;

            FileUploaded.Invoke(this, new EventArgs());
        }

        private string UploadedFilesText
        {
            get => (string)ViewState[nameof(UploadedFilesText)];
            set
            {
                ViewState[nameof(UploadedFilesText)] = value;
                _isMetadataInited = false;
            }
        }

        private UploadMetadata _metadata;
        private bool _isMetadataInited;

        public UploadMetadata Metadata
        {
            get
            {
                if (!_isMetadataInited)
                {
                    try
                    {
                        _metadata = UploadedFilesText.IsNotEmpty()
                            ? ServiceLocator.Serializer.Deserialize<UploadMetadata>(UploadedFilesText)
                            : null;
                    }
                    catch
                    {
                        _metadata = null;
                    }

                    _isMetadataInited = true;
                }

                return _metadata;
            }
        }

        public void ClearMetadata()
        {
            UploadedFilesText = null;
        }

        public string FilePath => Metadata?.FilePath;

        public long FileSize => HasFile ? new FileInfo(Metadata.FilePath).Length : -1;

        public bool HasFile => Metadata != null;

        public Stream OpenFile()
        {
            return HasFile
                ? IoFile.Open(Metadata.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                : Stream.Null;
        }

        public string ReadFileText(Encoding encoding)
        {
            return HasFile
                ? IoFile.ReadAllText(Metadata.FilePath, encoding)
                : throw new ApplicationError("File is not uploaded");
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public bool AllowMultiple
        {
            get => ViewState[nameof(AllowMultiple)] as bool? ?? false;
            set => ViewState[nameof(AllowMultiple)] = value;
        }

        public string LabelText
        {
            get => (string)ViewState[nameof(LabelText)] ?? "Select and Upload File(s):";
            set => ViewState[nameof(LabelText)] = value;
        }

        public string InputText
        {
            get => (string)ViewState[nameof(InputText)];
            set => ViewState[nameof(InputText)] = value;
        }

        public string ContainerType
        {
            get => (string)ViewState[nameof(ContainerType)] ?? "file";
            set => ViewState[nameof(ContainerType)] = value;
        }

        public Guid ContainerIdentifier
        {
            get => (Guid)(ViewState[nameof(ContainerIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(ContainerIdentifier)] = value;
        }

        public FileUploadType FileUploadType
        {
            get => (FileUploadType?)ViewState[nameof(FileUploadType)] ?? FileUploadType.Document;
            set => ViewState[nameof(FileUploadType)] = value;
        }

        public int? MaxFileSize
        {
            get => (int?)ViewState[nameof(MaxFileSize)];
            set => ViewState[nameof(MaxFileSize)] = value;
        }

        public int? MaxFileNameLength
        {
            get => (int?)ViewState[nameof(MaxFileNameLength)];
            set => ViewState[nameof(MaxFileNameLength)] = value;
        }

        public Guid? ResponseSessionIdentifier
        {
            get => (Guid?)ViewState[nameof(ResponseSessionIdentifier)];
            set => ViewState[nameof(ResponseSessionIdentifier)] = value;
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] AllowedExtensions
        {
            get => (string[])ViewState[nameof(AllowedExtensions)];
            set => ViewState[nameof(AllowedExtensions)] = value;
        }

        public string OnClientFileUploaded
        {
            get => (string)ViewState[nameof(OnClientFileUploaded)];
            set => ViewState[nameof(OnClientFileUploaded)] = value;
        }

        public string OnClientFileUploadFailed
        {
            get => (string)ViewState[nameof(OnClientFileUploadFailed)];
            set => ViewState[nameof(OnClientFileUploadFailed)] = value;
        }

        public string SelectedButtonClientID => $"{ClientID}_SelectButton";
        public string SelectedFileNamesClientID => $"{ClientID}_SelectedFileNames";
        public string SelectedFilesClientID => $"{ClientID}_SelectFiles";
        public string UploadProgressClientID => $"{ClientID}_UploadProgress";
        public string UploadedFilesClientID => $"{ClientID}";

        #endregion

        #region Render & PreRender

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Visible)
                return;

            var settings = new Settings
            {
                SelectButtonId = SelectedButtonClientID,
                SelectFilesId = SelectedFilesClientID,
                SelectedFileNamesId = SelectedFileNamesClientID,
                UploadedFilesId = UploadedFilesClientID,
                UploadProgressId = UploadProgressClientID,
                ContainerType = ContainerType,
                ContainerIdentifier = ContainerIdentifier.ToString(),
                MaxFileSize = GetMaxFileSize(),
                MaxFileNameLength = GetMaxFileNameLength(),

                // /\.(gif|jpg|jpeg|png)$/gi
                AllowedExtensionsRegex = AllowedExtensions != null && AllowedExtensions.Length > 0
                    ? $"\\.({string.Join("|", AllowedExtensions.Select(x => x.Substring(1)))})$"
                    : null,

                OnClientFileUploaded = OnClientFileUploaded,
                UniqueId = FileUploaded != null ? UniqueID : null,
                SessionIdentifier = ResponseSessionIdentifier.HasValue ? ResponseSessionIdentifier.ToString() : string.Empty,
            };

            AddClientSideStringSettings(settings);

            var settingsJson = JsonHelper.SerializeJsObject(settings);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(FileUploadV1),
                "init_" + ClientID,
                $"inSite.common.fileUploadV1.init({settingsJson});",
                true);
        }

        private int? GetMaxFileSize()
        {
            if (MaxFileSize.HasValue)
                return MaxFileSize;

            var settings = CurrentSessionState.Identity.Organization.PlatformCustomization.UploadSettings;

            switch (FileUploadType)
            {
                case FileUploadType.Document:
                    return settings.Documents.MaximumFileSize;
                case FileUploadType.Image:
                    return settings.Images.MaximumFileSize;
                case FileUploadType.Unlimited:
                    return null;
                default:
                    throw new ArgumentException($"Unknown FileUploadType: {FileUploadType}");
            }
        }

        private int GetMaxFileNameLength()
        {
            const int maxSystemPathLength = 259;

            var tempFolderPath = UploadHelper.GetTempFolderForUser(ServiceLocator.FilePaths, Page.User.Identity.Name);
            var maxFileNameLength = maxSystemPathLength - tempFolderPath.Length - 1;

            return MaxFileNameLength.HasValue
                ? Math.Min(maxFileNameLength, MaxFileNameLength.Value)
                : maxFileNameLength;
        }

        private void AddClientSideStringSettings(Settings settings)
        {
            var strings = new ClientSideStringSettings
            {
                FileNameLengthExceeded = $"File name is too long. Max allowed length is {settings.MaxFileNameLength} characters."
            };

            settings.Strings = strings;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group file-upload");

            if (!Width.IsEmpty)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, $"width:{Width};");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            {
                if (!string.IsNullOrEmpty(LabelText))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Label);
                    writer.Write(LabelText);
                    writer.RenderEndTag();
                }

                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, SelectedFileNamesClientID);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-control");
                        writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, "background-color:white;");
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, InputText);
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();

                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, SelectedButtonClientID);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-outline-secondary border btn-icon");
                        writer.AddAttribute(HtmlTextWriterAttribute.Title, "Browse");
                        writer.RenderBeginTag(HtmlTextWriterTag.Button);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "far fa-search");
                        writer.RenderBeginTag(HtmlTextWriterTag.I);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag();
                }

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Style, "margin-top: 5px;");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "progress");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, UploadProgressClientID);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "progress-bar");
                        writer.AddAttribute("role", "progressbar");
                        writer.AddAttribute("aria-valuenow", "0");
                        writer.AddAttribute("aria-valuemin", "0");
                        writer.AddAttribute("aria-valuemax", "100");
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, "width: 0%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag();
                }

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none;");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "file");
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, SelectedFilesClientID);

                    if (AllowMultiple)
                        writer.AddAttribute("multiple", "");

                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();

                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, UploadedFilesClientID);
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);

                    if (!string.IsNullOrEmpty(UploadedFilesText))
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, UploadedFilesText);

                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        #endregion

        #region IPostBackDataHandler & IPostBackEventHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            var newValue = postCollection[postDataKey];
            if (string.Equals(newValue, UploadedFilesText))
                return false;

            UploadedFilesText = newValue;

            return true;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {

        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (!Visible)
                return;

            Page.ClientScript.ValidateEvent(UniqueID, eventArgument);

            OnFileUploaded();
        }

        #endregion
    }
}