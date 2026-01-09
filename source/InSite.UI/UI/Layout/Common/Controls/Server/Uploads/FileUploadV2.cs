using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Files.Read;

using Shift.Common;
using Shift.Common.File;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(FileName))]
    public class FileUploadV2 : Control, IPostBackDataHandler, IPostBackEventHandler
    {
        #region Constants

        private static readonly string[] DefaultAllowedExtensions = new string[]
        {
            ".png",
            ".gif",
            ".jpg",
            ".jpeg",
            ".doc",
            ".docx",
            ".ppt",
            ".pptx",
            ".xls",
            ".xlsx",
            ".txt",
            ".pdf",
            ".zip"
        };

        #endregion

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
            public int? MaxFileSize { get; set; }
            public int MaxFileNameLength { get; set; }
            public string AllowedExtensionsRegex { get; set; }
            public string AllowedExtensionsText { get; set; }
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

            InputText = FileName;

            FileUploaded.Invoke(this, new EventArgs());
        }

        private string UploadedFilesText
        {
            get => (string)ViewState[nameof(UploadedFilesText)];
            set
            {
                ViewState[nameof(UploadedFilesText)] = value;
                _filesInited = false;
            }
        }

        private UploadFileInfo[] _files;
        private bool _filesInited;

        private UploadFileInfo[] Files
        {
            get
            {
                if (!_filesInited)
                {
                    try
                    {
                        _files = UploadedFilesText.IsNotEmpty()
                            ? ServiceLocator.Serializer.Deserialize<UploadFileInfo[]>(UploadedFilesText)
                            : null;
                    }
                    catch
                    {
                        _files = null;
                    }

                    _filesInited = true;
                }

                return _files;
            }
        }

        public UploadFileInfo File => HasFile ? Files[0] : null;

        public bool HasFile => Files != null && Files.Length > 0;

        public string FileName => File?.FileName;

        public int FileSize => File?.FileSize ?? -1;

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
            get => (string[])ViewState[nameof(AllowedExtensions)] ?? DefaultAllowedExtensions;
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

        #region File manipulation methods

        public void ClearFiles()
        {
            UploadedFilesText = null;
            InputText = null;
        }

        public void DeleteFile()
        {
            if (!HasFile)
                return;

            ServiceLocator.StorageService.Delete(File.FileIdentifier);

            ClearFiles();
        }

        public Stream OpenFile()
        {
            if (!HasFile)
                return Stream.Null;

            var (_, stream) = ServiceLocator.StorageService.GetFileStream(File.FileIdentifier);

            return stream;
        }

        public string ReadFileText(Encoding encoding)
        {
            if (!HasFile)
                throw new ApplicationError("File is not uploaded");

            var (_, stream) = ServiceLocator.StorageService.GetFileStream(File.FileIdentifier);

            using (var reader = new StreamReader(stream, encoding))
                return reader.ReadToEnd();
        }

        public FileStorageModel SaveFile(Guid objectIdentifier, FileObjectType objectType, string fileName = null)
        {
            return SaveFile(File, objectIdentifier, objectType, fileName);
        }

        public static FileStorageModel SaveFile(UploadFileInfo fileInfo, Guid objectIdentifier, FileObjectType objectType, string fileName)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            return SaveFile(fileInfo.FileIdentifier, objectIdentifier, objectType, fileName);
        }

        public static FileStorageModel SaveFile(FileStorageModel fileModel, Guid objectIdentifier, FileObjectType objectType, string fileName)
        {
            if (fileModel == null)
                throw new ArgumentNullException(nameof(fileModel));

            return SaveFile(fileModel.FileIdentifier, objectIdentifier, objectType, fileName);
        }

        private static FileStorageModel SaveFile(Guid fileIdentifier, Guid objectIdentifier, FileObjectType objectType, string fileName)
        {
            var model = ServiceLocator.StorageService.GetFile(fileIdentifier)
                ?? throw new ApplicationException($"The file ID = '{fileIdentifier}' does not exist");

            if (model.ObjectType != FileObjectType.Temporary)
                throw new ArgumentException($"Object type is not temporary for the file ID = '{fileIdentifier}'");

            ServiceLocator.StorageService.ChangeObject(fileIdentifier, objectIdentifier, objectType);

            if (!string.IsNullOrEmpty(fileName) && !string.Equals(model.FileName, fileName))
            {
                var userId = CurrentSessionState.Identity.User?.Identifier ?? Shift.Constant.UserIdentifiers.Someone;
                ServiceLocator.StorageService.RenameFile(fileIdentifier, userId, fileName);
            }

            return ServiceLocator.StorageService.GetFile(fileIdentifier);
        }

        public static bool CanSaveFile(Guid fileIdentifier)
        {
            var model = ServiceLocator.StorageService.GetFile(fileIdentifier);
            return model != null && model.ObjectType == FileObjectType.Temporary;
        }

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
                MaxFileSize = GetMaxFileSize(),
                MaxFileNameLength = GetMaxFileNameLength(),

                // /\.(gif|jpg|jpeg|png)$/gi
                AllowedExtensionsRegex = AllowedExtensions != null && AllowedExtensions.Length > 0
                    ? $"\\.({string.Join("|", AllowedExtensions.Select(x => x.Substring(1)))})$"
                    : null,

                AllowedExtensionsText = AllowedExtensions != null && AllowedExtensions.Length > 0
                    ? string.Join(" ", AllowedExtensions)
                    : null,

                OnClientFileUploaded = OnClientFileUploaded,
                UniqueId = FileUploaded != null ? UniqueID : null,
                SessionIdentifier = ResponseSessionIdentifier.HasValue ? ResponseSessionIdentifier.ToString() : string.Empty,
            };

            AddClientSideStringSettings(settings);

            var settingsJson = JsonHelper.SerializeJsObject(settings);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(FileUploadV2),
                "init_" + ClientID,
                $"inSite.common.fileUploadV2.init({settingsJson});",
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

                        var btnClass = "btn btn-outline-secondary border btn-icon";

                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, SelectedButtonClientID);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, btnClass);
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
                        var progress = HasFile ? "100" : "0";

                        writer.AddAttribute(HtmlTextWriterAttribute.Id, UploadProgressClientID);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "progress-bar");
                        writer.AddAttribute("role", "progressbar");
                        writer.AddAttribute("aria-valuenow", progress);
                        writer.AddAttribute("aria-valuemin", "0");
                        writer.AddAttribute("aria-valuemax", "100");
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, $"width: {progress}%");
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

        #region Image helpers

        /// <summary>
        /// Adjusts the uploaded image and saves it as a new stored file under the given object,
        /// returning the created FileStorageModel. Returns null if there is no file.
        /// </summary>
        public FileStorageModel AdjustImageAndSave(Guid objectIdentifier, FileObjectType objectType, int width, int height)
        {
            return AdjustImageAndSave(objectIdentifier, objectType, width, height, ImageType.Jpeg, true, null, out _);
        }

        /// <summary>
        /// If file is missing or something fails early, returns null.
        /// </summary>
        public FileStorageModel AdjustImageAndSave(
            Guid objectIdentifier,
            FileObjectType objectType,
            int width,
            int height,
            ImageType format,
            bool keepAspectRatio,
            string overrideFileName,
            out List<string> messagesOut)
        {
            messagesOut = null;

            if (!HasFile)
                return null;

            var uploaded = File;
            if (uploaded == null)
                return null;

            var fileName = !string.IsNullOrEmpty(overrideFileName) ? overrideFileName : uploaded.FileName;

            var userId = CurrentSessionState.Identity.User != null
                ? CurrentSessionState.Identity.User.Identifier
                : Shift.Constant.UserIdentifiers.Someone;

            var orgId = CurrentSessionState.Identity.Organization != null
                ? CurrentSessionState.Identity.Organization.Identifier
                : OrganizationIdentifiers.Global;

            using (var input = OpenFile())
            {
                if (input == Stream.Null)
                    return null;

                using (var output = new MemoryStream())
                {
                    var messages = new List<string>();
                    ImageHelper.AdjustImage(
                        input,
                        output,
                        format, 
                        keepAspectRatio,
                        messages,
                        width,
                        height);

                    output.Position = 0;

                    var model = ServiceLocator.StorageService.Create(
                        output,
                        fileName,
                        orgId,
                        userId,
                        objectIdentifier,
                        objectType,
                        new FileProperties { DocumentName = fileName },
                        new List<FileClaim>());

                    messagesOut = messages;

                    return model;
                }
            }
        }


        /// <summary>
        /// Deletes a stored file based on its public URL. 
        /// Does nothing if URL is null/empty or parsing fails.
        /// </summary>
        public static void DeleteFileByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            var parsed = ServiceLocator.StorageService.ParseFileUrl(url);

            var fileIdentifier = parsed.Item1;
            if (fileIdentifier.HasValue)
                ServiceLocator.StorageService.Delete(fileIdentifier.Value);
        }

        /// <summary>
        /// Processes and returns the final public URL or null.
        /// </summary>
        public string AdjustImageSaveAndGetUrl(Guid objectIdentifier, FileObjectType objectType, int width, int height)
        {
            var model = AdjustImageAndSave(objectIdentifier, objectType, width, height);
            return model != null
                ? ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName)
                : null;
        }

        #endregion

    }
}