using System;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(false), PersistChildren(false)]
    public class MarkdownHtmlEditor : Control, INamingContainer
    {
        #region Properties

        public MarkdownHtmlMode DefaultMode
        {
            get => (MarkdownHtmlMode)(ViewState[nameof(DefaultMode)] ?? MarkdownHtmlMode.Text);
            set => ViewState[nameof(DefaultMode)] = value;
        }

        public string Title
        {
            get { return (string)ViewState[nameof(Title)]; }
            set { ViewState[nameof(Title)] = value; }
        }

        public string TextDescription
        {
            get
            {
                return (string)ViewState[nameof(TextDescription)];
            }
            set
            {
                ViewState[nameof(TextDescription)] = value;

                EnsureChildControls();

                var textField = _control.GetField(ContentSectionDefault.BodyText.ToString());
                textField.Description = value;
            }
        }

        public string HtmlDescription
        {
            get
            {
                return (string)ViewState[nameof(HtmlDescription)];
            }
            set
            {
                ViewState[nameof(HtmlDescription)] = value;

                EnsureChildControls();

                var htmlField = _control.GetField(ContentSectionDefault.BodyHtml.ToString());
                htmlField.Description = value;
            }
        }

        public bool HtmlDisabled
        {
            get
            {
                return ((bool?)ViewState[nameof(HtmlDisabled)]) ?? false;
            }
            set
            {
                ViewState[nameof(HtmlDisabled)] = value;
            }
        }

        public bool Required
        {
            get
            {
                return (bool)(ViewState[nameof(Required)] ?? false);
            }
            set
            {
                ViewState[nameof(Required)] = value;

                EnsureChildControls();

                var htmlField = _control.GetField(ContentSectionDefault.BodyHtml.ToString());
                htmlField.Required = value;

                var textField = _control.GetField(ContentSectionDefault.BodyText.ToString());
                textField.Required = value;
            }
        }

        public string Language
        {
            get
            {
                return (string)(base.ViewState[nameof(Language)] ?? Shift.Common.ContentContainer.DefaultLanguage);
            }
            set
            {
                var lang = value.IsEmpty() || !Shift.Common.Language.CodeExists(value) ? null : value;

                ViewState[nameof(Language)] = lang;

                EnsureChildControls();

                _control.SetLanguage(lang);
            }
        }

        public MultilingualString Html
        {
            get
            {
                EnsureChildControls();
                return _control.GetValue(ContentSectionDefault.BodyHtml.ToString());
            }
            set
            {
                EnsureChildControls();
                _control.GetField(ContentSectionDefault.BodyHtml.ToString()).Translation = value;
            }
        }

        public MultilingualString Text
        {
            get
            {
                EnsureChildControls();
                return _control.GetValue(ContentSectionDefault.BodyText.ToString());
            }
            set
            {
                EnsureChildControls();
                _control.GetField(ContentSectionDefault.BodyText.ToString()).Translation = value;
            }
        }

        public bool TranslateDisabled
        {
            get { return ((bool?)ViewState[nameof(TranslateDisabled)]) ?? false; }
            set { ViewState[nameof(TranslateDisabled)] = value; }
        }

        public UploadMode UploadMode
        {
            get { return (UploadMode?)ViewState[nameof(UploadMode)] ?? UploadMode.Advanced; }
            set { ViewState[nameof(UploadMode)] = value; }
        }

        public string UploadPath
        {
            get { return (string)ViewState[nameof(UploadPath)]; }
            set { ViewState[nameof(UploadPath)] = value; }
        }

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        private bool IsInited
        {
            get => (bool)(ViewState[nameof(IsInited)] ?? false);
            set => ViewState[nameof(IsInited)] = value;
        }

        #endregion

        #region Fields

        private SectionMdHtml _control = null;

        #endregion

        #region Initialization

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            var options = new AssetContentSection.MarkdownAndHtml("mdhtml");

            Controls.Add(_control = (SectionMdHtml)Page.LoadControl(options.ControlPath));

            if (!IsInited)
            {
                var htmlField = _control.GetField(ContentSectionDefault.BodyHtml.ToString());
                htmlField.AllowUpload = UploadPath.IsNotEmpty();
                htmlField.UploadFolderPath = UploadPath;
                htmlField.UploadMode = UploadMode;
                htmlField.Title = Title;

                var textField = _control.GetField(ContentSectionDefault.BodyText.ToString());
                textField.AllowUpload = UploadPath.IsNotEmpty();
                textField.UploadFolderPath = UploadPath;
                textField.UploadMode = UploadMode;
                textField.Title = Title;

                options.Title = Title;
                options.HtmlLabel = Title;
                options.MarkdownLabel = Title;

                options.MarkdownDescription = TextDescription;
                options.HtmlDescription = HtmlDescription;
                options.IsRequired = Required;
                options.AllowUpload = UploadPath.IsNotEmpty();
                options.UploadFolderPath = UploadPath;
                options.UploadStrategy = UploadMode;
                options.IsMultiValue = true;

                _control.HtmlDisabled = HtmlDisabled;
                _control.TranslateDisabled = TranslateDisabled;

                _control.SetValidationGroup(ValidationGroup);
                _control.SetLanguage(Language);
                _control.SetOptions(options);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsInited)
            {
                if (DefaultMode == MarkdownHtmlMode.Html)
                    _control.SelectEditor(ContentSectionDefault.BodyHtml);
                else if (DefaultMode == MarkdownHtmlMode.Text)
                    _control.SelectEditor(ContentSectionDefault.BodyText);

                IsInited = true;
            }

            _control.SetValidationGroup(ValidationGroup);

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "md-html-editor");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.Render(writer);

            writer.RenderEndTag();
        }

        #endregion
    }
}