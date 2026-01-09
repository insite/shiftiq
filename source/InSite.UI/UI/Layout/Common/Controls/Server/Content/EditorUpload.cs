using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.Infrastructure;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(false)]
    public class EditorUpload : Control
    {
        #region Constants

        private static readonly string[] DefaultFileExtensions = new[] { "png", "gif", "jpg", "jpeg", "doc", "docx", "ppt", "pptx", "xls", "xlsx", "txt", "pdf", "zip" };

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsInitSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ControlID { get; set; }

            [JsonProperty(PropertyName = "extensions")]
            public string Extensions { get; set; }

            [JsonProperty(PropertyName = "dCont")]
            public string DropContainer { get; set; }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CallbackData
        {
            [JsonProperty(PropertyName = "func")]
            public string Function { get; set; }

            [JsonProperty(PropertyName = "args", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public object Arguments { get; set; }
        }

        #endregion

        #region Events (Success)

        public class SuccessEventArgs : EventArgs
        {
            #region Properties

            public string Url { get; }
            public string Title { get; }
            public string[] Messages { get; }

            public CallbackData Callback { get; }

            #endregion

            #region Construction

            public SuccessEventArgs(string url, string title, string[] messages)
            {
                Url = url;
                Title = title;
                Messages = messages;

                Callback = new CallbackData();
            }

            #endregion
        }

        public delegate void SuccessEventHandler(object sender, SuccessEventArgs args);

        public event SuccessEventHandler Success;

        private SuccessEventArgs OnSuccess(string url, string title, string[] messages)
        {
            var args = new SuccessEventArgs(url, title, messages);

            Success?.Invoke(this, args);

            return args;
        }

        #endregion

        #region Events (Custom)

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CustomEventArgs : EventArgs
        {
            #region Properties

            public HttpPostedFile File { get; }

            public CallbackData Callback { get; }

            #endregion

            #region Construction

            public CustomEventArgs(HttpPostedFile file)
            {
                File = file;

                Callback = new CallbackData();
            }

            #endregion
        }

        public delegate void CustomEventHandler(object sender, CustomEventArgs args);

        public event CustomEventHandler Custom;

        private CustomEventArgs OnCustom(HttpPostedFile file)
        {
            var args = new CustomEventArgs(file);

            Custom?.Invoke(this, args);

            return args;
        }

        #endregion

        #region Properties

        public UploadMode Mode
        {
            get => (UploadMode)(ViewState[nameof(Mode)] ?? UploadMode.Advanced);
            set => ViewState[nameof(Mode)] = value;
        }

        public string FolderPath
        {
            get => (string)ViewState[nameof(FolderPath)];
            set => ViewState[nameof(FolderPath)] = value;
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] FileExtensions
        {
            get => (string[])ViewState[nameof(FileExtensions)] ?? DefaultFileExtensions;
            set => ViewState[nameof(FileExtensions)] = value;
        }

        public int MaxFileSizeImage
        {
            get => (int)(ViewState[nameof(MaxFileSizeImage)] ?? CurrentSessionState.Identity.Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize);
            set => ViewState[nameof(MaxFileSizeImage)] = value;
        }

        public int MaxFileSizeDocument
        {
            get => (int)(ViewState[nameof(MaxFileSizeDocument)] ?? CurrentSessionState.Identity.Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize);
            set => ViewState[nameof(MaxFileSizeDocument)] = value;
        }

        public string DropContainer
        {
            get => (string)ViewState[nameof(DropContainer)];
            set => ViewState[nameof(DropContainer)] = value;
        }

        #endregion

        #region Fields

        private FileUpload _fileUpload;
        private HtmlInputButton _uploadButton;
        private RequiredValidator _requiredValidator;
        private FileExtensionValidator _extensionValidator;
        private CustomValidator _imageSizeValidator;
        private CustomValidator _documentSizeValidator;

        #endregion

        #region Initialization

        protected override void CreateChildControls()
        {
            Controls.Add(_fileUpload = new FileUpload
            {
                ID = ID + "_upl"
            });
            Controls.Add(_uploadButton = new HtmlInputButton
            {
                ID = ID + "_btn",
                CausesValidation = true
            });
            Controls.Add(_requiredValidator = new RequiredValidator
            {
                ControlToValidate = _fileUpload.ID,
                FieldName = "File",
                Display = ValidatorDisplay.None
            });
            Controls.Add(_extensionValidator = new FileExtensionValidator
            {
                ControlToValidate = _fileUpload.ID,
                EnableClientScript = false,
                Display = ValidatorDisplay.None
            });
            Controls.Add(_imageSizeValidator = new CustomValidator
            {
                ControlToValidate = _fileUpload.ID,
                EnableClientScript = false,
                Display = ValidatorDisplay.None
            });
            Controls.Add(_documentSizeValidator = new CustomValidator
            {
                ControlToValidate = _fileUpload.ID,
                EnableClientScript = false,
                Display = ValidatorDisplay.None
            });
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            _uploadButton.ServerClick += UploadFileButton_Click;
            _imageSizeValidator.ServerValidate += ImageSizeValidator_ServerValidate;
            _documentSizeValidator.ServerValidate += DocumentSizeValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var validationGroup = ClientID;

            _uploadButton.ValidationGroup = validationGroup;
            _requiredValidator.ValidationGroup = validationGroup;
            _extensionValidator.ValidationGroup = validationGroup;
            _imageSizeValidator.ValidationGroup = validationGroup;
            _documentSizeValidator.ValidationGroup = validationGroup;

            SetupValidators();
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetupValidators();

            var options = new JsInitSettings
            {
                ControlID = ClientID,
                Extensions = _extensionValidator.Enabled
                    ? string.Join(" ", _extensionValidator.FileExtensions.Select(x => "." + x))
                    : null,
                DropContainer = DropContainer,
            };

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "refresh_" + ClientID,
                $"inSite.common.editorUpload.init({JsonHelper.SerializeJsObject(options)});", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void ImageSizeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var maxSize = MaxFileSizeImage;

            args.IsValid = maxSize <= 0
                || !_fileUpload.HasFile
                || !FileExtension.IsImage(_fileUpload.FileName)
                || _fileUpload.PostedFile.ContentLength <= maxSize;
        }

        private void DocumentSizeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var maxSize = MaxFileSizeDocument;

            args.IsValid = maxSize <= 0
                || !_fileUpload.HasFile
                || FileExtension.IsImage(_fileUpload.FileName)
                || _fileUpload.PostedFile.ContentLength <= maxSize;
        }

        private void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                var messages = Page.Validators
                    .Cast<IValidator>().Where(v => !v.IsValid).Select(v => v.ErrorMessage)
                    .ToArray();

                SendError(HttpStatusCode.BadRequest, messages);

                return;
            }

            try
            {
                CallbackData callback;

                if (Mode == UploadMode.Basic)
                {
                    if (FolderPath.IsEmpty())
                        throw new ApplicationError("FolderPath is null");

                    var upload = Api.Controllers.FilesLegacyController
                        .SaveUploadedFile("Uploads", Guid.NewGuid(), _fileUpload.PostedFile, CurrentSessionState.Identity.User.Email);

                    var url = UploadHelper
                        .SaveUploadedFiles(ServiceLocator.FilePaths, upload, FolderPath);

                    callback = OnSuccess(url, upload.FileName, null).Callback;
                }
                else if (Mode == UploadMode.Advanced)
                {
                    if (FolderPath.IsEmpty())
                        throw new ApplicationError("FolderPath is null");

                    var file = _fileUpload.PostedFile;
                    var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                    var fileExt = Path.GetExtension(file.FileName);

                    var path = FolderPath;
                    if (!path.EndsWith("/"))
                        path += "/";
                    path += fileName + fileExt;

                    var document = FileHelper.Provider.Save(CurrentSessionState.Identity.Organization.Identifier, path, file.InputStream);

                    callback = OnSuccess(FileHelper.GetUrl(document.Path), document.Name, document.ActionMessages.ToArray()).Callback;
                }
                else if (Mode == UploadMode.Custom)
                {
                    callback = OnCustom(_fileUpload.PostedFile).Callback;
                }
                else
                {
                    throw new ApplicationError("Unexpected upload mode: " + Mode.GetName());
                }

                if (callback.Function.IsNotEmpty())
                    SendResponse(JsonConvert.SerializeObject(callback));
                else
                    SendResponse("{}");
            }
            catch (ApplicationError kex)
            {
                SendError(HttpStatusCode.InternalServerError, kex.Message);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                SendError(HttpStatusCode.InternalServerError, "An error occurred on the server side");
            }

            void SendError(HttpStatusCode status, params string[] messages)
            {
                SendResponse(JsonConvert.SerializeObject(messages), status);
            }

            void SendResponse(string data, HttpStatusCode status = HttpStatusCode.OK)
            {
                var response = Context.Response;
                response.Clear();
                response.StatusCode = (int)status;
                response.ContentType = "application/json";
                response.Write(data);
                response.End();
            }
        }

        #endregion

        #region Methods

        private void SetupValidators()
        {
            _imageSizeValidator.ErrorMessage = FileStorage.MaxFileSizeExceededException.GetMessage("image", MaxFileSizeImage);
            _imageSizeValidator.Enabled = MaxFileSizeImage > 0;

            _documentSizeValidator.ErrorMessage = FileStorage.MaxFileSizeExceededException.GetMessage("document", MaxFileSizeDocument);
            _documentSizeValidator.Enabled = MaxFileSizeDocument > 0;

            _extensionValidator.Enabled = FileExtensions.Length > 0;
            _extensionValidator.FileExtensions = FileExtensions.Select(x => x.Trim()).ToArray();
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "d-none");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.Render(writer);

            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        #endregion
    }
}