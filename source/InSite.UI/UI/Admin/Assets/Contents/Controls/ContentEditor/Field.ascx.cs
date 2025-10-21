using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assets.Contents.Controls.ContentEditor
{
    public partial class Field : BaseUserControl
    {
        #region Enum

        public enum ContentType
        {
            SinglelineText,
            MultilineText,
            Markdown,
            Html
        }

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsInitSettings
        {
            [JsonProperty(PropertyName = "textId")]
            public string TextID { get; set; }

            [JsonProperty(PropertyName = "cleanTextId")]
            public string CleanTextID { get; set; }
        }

        #endregion

        #region Events

        public event EditorTranslation.RequestEventHandler TranslationRequested
        {
            add => EditorTranslation.Requested += value;
            remove => EditorTranslation.Requested -= value;
        }

        #endregion

        #region Properties

        public ContentType InputContent
        {
            get => (ContentType)(ViewState[nameof(InputContent)] ?? ContentType.MultilineText);
            set => ViewState[nameof(InputContent)] = value;
        }

        public string InputLanguage
        {
            get => EditorTranslation.Language;
            set => EditorTranslation.Language = value;
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

        public string Description
        {
            get => (string)ViewState[nameof(Description)];
            set => ViewState[nameof(Description)] = value;
        }

        public MultilingualString Translation
        {
            get => EditorTranslation.Text;
            set => EditorTranslation.Text = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool TranslateDisabled
        {
            get => EditorTranslation.TranslateDisabled;
            set => EditorTranslation.TranslateDisabled = value;
        }

        public bool Required
        {
            get => ClientStateRequiredValidator.Enabled;
            set => ClientStateRequiredValidator.Enabled = value;
        }

        public bool AllowUpload
        {
            get => EditorUpload.Visible;
            set => EditorUpload.Visible = value;
        }

        public string UploadFolderPath
        {
            get => EditorUpload.FolderPath;
            set => EditorUpload.FolderPath = value;
        }

        public UploadMode UploadMode
        {
            get => EditorUpload.Mode;
            set => EditorUpload.Mode = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonScript.ContentKey = GetType().FullName;

            ClientStateRequiredValidator.ServerValidate += ClientStateRequiredValidator_ServerValidate;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            ClientStateRequiredValidator.ValidationGroup = groupName;
        }

        public void SetupInput(ContentType type)
        {
            Input.UnloadControl();

            var isHtml = type == ContentType.Html;
            var isMarkdown = type == ContentType.Markdown;

            CleanTextButton.Visible = isHtml;
            EditorTranslation.EnableMarkdownConverter = isMarkdown;
            InputContent = type;

            if (type == ContentType.SinglelineText)
            {
                var text = Input.LoadControl<Common.Web.UI.TextBox>();
                text.Width = Unit.Percentage(100);
                text.TranslationControl = EditorTranslation.ID;
            }
            else if (type == ContentType.MultilineText)
            {
                var text = Input.LoadControl<Common.Web.UI.TextBox>();
                text.Width = Unit.Percentage(100);
                text.TextMode = Shift.Constant.TextBoxMode.MultiLine;
                text.TranslationControl = EditorTranslation.ID;
            }
            else if (isHtml)
            {
                var text = Input.LoadControl<HtmlEditor>();
                text.TranslationControl = EditorTranslation.ID;
                text.UploadControl = EditorUpload.ID;
            }
            else if (isMarkdown)
            {
                var text = Input.LoadControl<MarkdownEditor>();
                text.TranslationControl = EditorTranslation.ID;
                text.UploadControl = EditorUpload.ID;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!Input.HasControl())
                SetupInput(InputContent);

            var options = new JsInitSettings
            {
                TextID = Input.GetControl().ClientID,
                CleanTextID = CleanTextButton.ClientID,
            };

            var title = HttpUtility.HtmlEncode(Title);

            TitleOutput.Visible = Title.HasValue();
            TitleOutput.Text = title;
            TitleLabel.Visible = TitleOutput.Visible;
            TitleLabel.Attributes["class"] = "form-label";

            ScriptManager.RegisterStartupScript(Page, GetType(), "refresh_" + ClientID, $"contentEditorField.init({JsonHelper.SerializeJsObject(options)});", true);

            base.OnPreRender(e);
        }

        public void SetOptions(AssetContentSection options)
        {
            if (options is AssetContentSection.SingleLine)
            {
                SetupInput(ContentType.SinglelineText);
            }
            else if (options is AssetContentSection.Markdown markdown)
            {
                SetupInput(ContentType.Markdown);
                AllowUpload = markdown.AllowUpload;
                UploadFolderPath = markdown.UploadFolderPath;
                UploadMode = markdown.UploadStrategy;
            }
            else if (options is AssetContentSection.Html html)
            {
                SetupInput(ContentType.Html);
                AllowUpload = html.AllowUpload;
                UploadFolderPath = html.UploadFolderPath;
                UploadMode = html.UploadStrategy;
            }
            else
            {
                throw new NotImplementedException("Pill type: " + options.GetType().FullName);
            }

            var textOptions = (AssetContentSection.TextSection)options;
            Title = textOptions.Label;
            Description = textOptions.Description;
            Required = textOptions.IsRequired;
            Translation = textOptions.Value ?? new MultilingualString();
        }

        #endregion

        #region Event handlers

        private void ClientStateRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = EditorTranslation.Text.Any(x => !string.IsNullOrWhiteSpace(x.Value));
        }

        #endregion
    }
}