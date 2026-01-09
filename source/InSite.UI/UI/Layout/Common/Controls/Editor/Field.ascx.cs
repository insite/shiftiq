using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using TextBoxMode = Shift.Constant.TextBoxMode;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class Field : BaseUserControl, IHasToolTip
    {
        #region Constants

        private const string DefaultLanguage = "en";

        #endregion

        #region Enums

        public enum ContentTypeEnum
        {
            Text,
            Markdown,
            Html
        }

        #endregion

        #region Events

        public class TranslationRequestEventArgs : EventArgs
        {
            #region Properties

            public string FromLanguage { get; }

            public string[] ToLanguages { get; }

            #endregion

            #region Construction

            public TranslationRequestEventArgs(string fromLanguage, string[] toLanguages)
            {
                FromLanguage = fromLanguage;
                ToLanguages = toLanguages;
            }

            #endregion
        }

        public delegate void TranslationRequestEventHandler(object sender, TranslationRequestEventArgs args);

        public event TranslationRequestEventHandler TranslationRequested;

        private void OnTranslationRequested(string fromLang, string[] toLangs) =>
            TranslationRequested?.Invoke(this, new TranslationRequestEventArgs(fromLang, toLangs));

        #endregion

        #region Classes

        [Serializable]
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class StateInfo
        {
            [JsonProperty(PropertyName = "lang")]
            public string Language
            {
                get
                {
                    return _lang;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value) && Shift.Common.Language.CodeExists(value))
                        _lang = value;
                }
            }

            [JsonProperty(PropertyName = "data")]
            public MultilingualString Data { get; set; }

            private string _lang;

            public StateInfo()
            {
                Language = CurrentSessionState.Identity.Language;
            }

            internal void EnsureOrganizationLanguages()
            {
                if (Data == null)
                    Data = new MultilingualString();

                foreach (var language in Organization.Languages.Select(x => x.Name))
                {
                    if (!Data.Languages.Contains(language))
                        Data[language] = string.Empty;
                }
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsInitSettings
        {
            [JsonProperty(PropertyName = "langId")]
            public string LanguageOutputID { get; set; }

            [JsonProperty(PropertyName = "textId")]
            public string TranslationTextID { get; set; }

            [JsonProperty(PropertyName = "stateId")]
            public string StateInputID { get; set; }

            [JsonProperty(PropertyName = "type")]
            public string ContentType { get; set; }

            [JsonProperty(PropertyName = "state")]
            public string CurrentState { get; set; }

            [JsonProperty(PropertyName = "upload")]
            public bool AllowUpload { get; set; }
        }

        private interface IJsonResult
        {
            string Type { get; }
        }

        private class UploadSuccessResult : IJsonResult
        {
            public UploadSuccessResult(string path, string name, bool isImage)
            {
                Type = "OK";
                Path = path;
                Name = name;
                IsImage = isImage;
                Messages = new List<string>();
            }

            public string Path { get; }
            public string Name { get; }
            public bool IsImage { get; }
            public string Type { get; }
            public List<string> Messages { get; }
        }

        private class UploadErrorResult : IJsonResult
        {
            public UploadErrorResult()
            {
                Type = "ERROR";
                Messages = new List<string>();
            }

            public UploadErrorResult(string message)
                : this()
            {
                Messages.Add(message);
            }

            public List<string> Messages { get; }
            public string Type { get; }
        }

        #endregion

        #region Properties

        private StateInfo ClientState
        {
            get => (StateInfo)(ViewState[nameof(ClientState)] ?? (ViewState[nameof(ClientState)] = new StateInfo()));
            set => ViewState[nameof(ClientState)] = value;
        }

        public string InputLanguage
        {
            get => ClientState.Language;
            set => ClientState.Language = value;
        }

        public string Title
        {
            get => (string)ViewState[nameof(Title)];
            set
            {
                ViewState[nameof(Title)] = value;

                ClientStateRequiredValidator.ErrorMessage = string.IsNullOrEmpty(value)
                    ? string.Empty
                    : $"Required field: {value}";
            }
        }

        public string ToolTip
        {
            get => (string)ViewState[nameof(ToolTip)];
            set => ViewState[nameof(ToolTip)] = value;
        }

        public string Description
        {
            get => (string)ViewState[nameof(Description)];
            set => ViewState[nameof(Description)] = value;
        }

        public MultilingualString Translation
        {
            get => ClientState.Data;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                ClientState.Data = value;
                ClientState.EnsureOrganizationLanguages();
            }
        }

        public string TranslationJson
        {
            get => ClientState.Data.Serialize();
            set => ClientState.Data = MultilingualString.Deserialize(value);
        }

        public bool Required
        {
            get => ClientStateRequiredValidator.Enabled;
            set => ClientStateRequiredValidator.Enabled = value;
        }

        public TextBoxMode InputTextMode
        {
            get => (TextBoxMode)(ViewState[nameof(InputTextMode)] ?? TextBoxMode.MultiLine);
            set => ViewState[nameof(InputTextMode)] = value;
        }

        public ContentTypeEnum ContentType
        {
            get => (ContentTypeEnum)(ViewState[nameof(ContentType)] ?? ContentTypeEnum.Text);
            set => ViewState[nameof(ContentType)] = value;
        }

        public bool AllowUpload
        {
            get => ViewState[nameof(AllowUpload)] != null && (bool)ViewState[nameof(AllowUpload)];
            set => ViewState[nameof(AllowUpload)] = value;
        }

        public string UploadFolderPath
        {
            get => (string)ViewState[nameof(UploadFolderPath)];
            set => ViewState[nameof(UploadFolderPath)] = value;
        }

        protected string TranslateToLanguagesString => string.Join(", ", TranslateToLanguages.Select(x => x.DisplayName));

        protected CultureInfo[] TranslateToLanguages
        {
            get
            {

                var value = (CultureInfo[])Context.Items[_translateToLanguagesKey];

                if (value == null)
                    Context.Items[_translateToLanguagesKey] = value = Organization.Languages.Where(x => x.Name != DefaultLanguage).ToArray();

                return value;
            }
        }

        #endregion

        #region Fields

        private static readonly string _translateToLanguagesKey = typeof(SectionBase).FullName + "." + nameof(TranslateToLanguages);

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = GetType().FullName;
            CommonScript.ContentKey = GetType().FullName;

            ClientStateRequiredValidator.ServerValidate += ClientStateRequiredValidator_ServerValidate;

            var uploadValidationGroup = $"{ClientID}_UploadFile";

            UploadFileButton.Click += UploadFileButton_Click;
            UploadFileButton.ValidationGroup = uploadValidationGroup;
            FileUploadRequiredValidator.ValidationGroup = uploadValidationGroup;
            FileUploadExtensionValidator.ValidationGroup = uploadValidationGroup;

            var limit = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;
            var maximumFileSizeException = new FileStorage.MaxFileSizeExceededException("image", limit);
            FileUploadImageSizeValidator.ErrorMessage = maximumFileSizeException.Message;
            FileUploadImageSizeValidator.Enabled = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize > 0;
            FileUploadImageSizeValidator.ServerValidate += FileUploadImageSizeValidator_ServerValidate;
            FileUploadImageSizeValidator.ValidationGroup = uploadValidationGroup;

            limit = Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize;
            maximumFileSizeException = new FileStorage.MaxFileSizeExceededException("document", limit);
            FileUploadDocumentSizeValidator.ErrorMessage = maximumFileSizeException.Message;
            FileUploadDocumentSizeValidator.Enabled = Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize > 0;
            FileUploadDocumentSizeValidator.ServerValidate += FileUploadDocumentSizeValidator_ServerValidate;
            FileUploadDocumentSizeValidator.ValidationGroup = uploadValidationGroup;

            RequestTranslationButton.Visible = TranslateToLanguages.Length > 0;
            RequestTranslationButton.Click += RequestTranslationButton_Click;
            RequestTranslationButton.OnClientClick = $"if (!confirm('Are you sure you want to translate this content from English to {TranslateToLanguagesString}?')) return false;";
        }

        protected override void SetupValidationGroup(string groupName)
        {
            ClientStateRequiredValidator.ValidationGroup = groupName;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!string.IsNullOrEmpty(StateInput.Value))
            {
                ClientState = JsonConvert.DeserializeObject<StateInfo>(StateInput.Value);
                ClientState.EnsureOrganizationLanguages();
            }

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (AllowUpload && string.IsNullOrEmpty(UploadFolderPath))
                throw new ApplicationError("UploadFolderPath is null");

            StateInput.Value = JsonConvert.SerializeObject(ClientState);

            var textMode = ContentType == ContentTypeEnum.Text ? InputTextMode : TextBoxMode.MultiLine;

            TranslationText.TextMode = textMode;

            var options = new JsInitSettings
            {
                LanguageOutputID = LanguageOutput.ClientID,
                TranslationTextID = TranslationText.ClientID,
                StateInputID = StateInput.ClientID,
                AllowUpload = AllowUpload
            };

            if (ContentType == ContentTypeEnum.Text)
                options.ContentType = "txt";
            else if (ContentType == ContentTypeEnum.Markdown)
                options.ContentType = "md";
            else if (ContentType == ContentTypeEnum.Html)
                options.ContentType = "html";
            else
                throw new ApplicationError("Content type is not implemented: " + ContentType.GetName());

            if (HttpRequestHelper.IsAjaxRequest)
                options.CurrentState = StateInput.Value;

            ScriptManager.RegisterStartupScript(Page, GetType(), "refresh_" + ClientID, $"contentEditorField.init({JsonHelper.SerializeJsObject(options)});", true);

            base.OnPreRender(e);
        }

        public void SetOptions(LayoutContentSection options)
        {
            if (options is LayoutContentSection.SingleLine singleLine)
            {
                ContentType = ContentTypeEnum.Text;
                InputTextMode = TextBoxMode.SingleLine;
            }
            else if (options is LayoutContentSection.Markdown markdown)
            {
                ContentType = ContentTypeEnum.Markdown;
                AllowUpload = markdown.AllowUpload;
                UploadFolderPath = markdown.UploadFolderPath;
            }
            else if (options is LayoutContentSection.Html html)
            {
                ContentType = ContentTypeEnum.Html;
                AllowUpload = html.AllowUpload;
                UploadFolderPath = html.UploadFolderPath;
            }
            else
            {
                throw new NotImplementedException("Pill type: " + options.GetType().FullName);
            }

            var textOptions = (LayoutContentSection.TextSection)options;
            Title = textOptions.Label;
            Description = textOptions.Description;
            ToolTip = textOptions.Tooltip;
            Required = textOptions.IsRequired;
            Translation = textOptions.Value ?? new MultilingualString();
        }

        #endregion

        #region Event handlers

        private void FileUploadImageSizeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !FileUpload.HasFile || !FileExtension.IsImage(FileUpload.FileName)
                || FileUpload.PostedFile.ContentLength <= Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;
        }

        private void FileUploadDocumentSizeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !FileUpload.HasFile || FileExtension.IsImage(FileUpload.FileName)
                || FileUpload.PostedFile.ContentLength <= Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize;
        }

        private void ClientStateRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ClientState.Data.Any(x => !string.IsNullOrWhiteSpace(x.Value));
        }

        private void RequestTranslationButton_Click(object sender, EventArgs e)
        {
            var fromLang = DefaultLanguage;
            var fromText = Translation[fromLang];

            if (string.IsNullOrEmpty(fromText))
            {
                ScriptManager.RegisterStartupScript(
                    Page,
                    GetType(),
                    "translation_message",
                    $"alert({HttpUtility.JavaScriptStringEncode("Error: the source language has no content.", true)});",
                    true);

                return;
            }

            var toLangs = TranslateToLanguages.Select(x => x.Name).ToArray();
            foreach (var toLang in toLangs)
                ((IHasTranslator)Page).Translator.Translate(fromLang, toLang, Translation);

            OnTranslationRequested(fromLang, toLangs);
        }

        private void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UploadFolderPath))
                return;

            IJsonResult result;

            if (!Page.IsValid)
            {
                var error = new UploadErrorResult();

                foreach (IValidator validator in Page.Validators)
                {
                    if (!validator.IsValid)
                        error.Messages.Add(validator.ErrorMessage);
                }

                result = error;
            }
            else
            {
                try
                {
                    var file = FileUpload.PostedFile;
                    var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                    var fileExt = Path.GetExtension(file.FileName);

                    var document = FileHelper.Provider.Save(
                        CurrentSessionState.Identity.Organization.Identifier,
                        UploadFolderPath + "/" + fileName + fileExt,
                        file.InputStream);
                    var url = HttpRequestHelper.CurrentRootUrlFiles + UploadFolderPath;

                    var successResult = new UploadSuccessResult(
                        url,
                        document.Name,
                        !string.IsNullOrEmpty(document.Type) && FileExtension.IsImage(document.Type));

                    foreach (var message in document.ActionMessages)
                    {
                        if (message.StartsWith("Warning: image resized"))
                        {
                            var sizeStartIndex = message.IndexOf('(') + 1;
                            var sizeEndIndex = message.IndexOf(')', sizeStartIndex);
                            var sizeStringParts = message.Substring(sizeStartIndex, sizeEndIndex - sizeStartIndex).Split(new[] { " -> " }, StringSplitOptions.None);

                            successResult.Messages.Add(
                                $"The recommended maximum size for an upload image is {sizeStringParts[1]} pixels." +
                                $"\r\nThe size of your image is {sizeStringParts[0]} pixels." +
                                $"\r\nThe system has automatically scaled the image for you, but you may want to resize your own images in the future before you upload them.");
                        }
                    }

                    result = successResult;
                }
                catch (ApplicationError kex)
                {
                    result = new UploadErrorResult(kex.Message);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);

                    result = new UploadErrorResult("An error occurred on the server side");
                }
            }

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(result));
            Response.End();
        }

        #endregion
    }
}