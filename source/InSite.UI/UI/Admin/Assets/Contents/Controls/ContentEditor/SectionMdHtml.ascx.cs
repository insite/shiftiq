using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assets.Contents.Controls.ContentEditor
{
    public partial class SectionMdHtml : SectionBase
    {
        #region Events

        public event TranslationRequestEventHandler TranslationRequested;

        private void OnTranslationRequested(Field.ContentType content, EditorTranslation.RequestEventArgs args) =>
            TranslationRequested?.Invoke(this, new TranslationRequestEventArgs(content, args.FromLanguage, args.ToLanguages));

        #endregion

        #region Properties

        public bool EnableAJAX
        {
            get => (bool)(ViewState[nameof(EnableAJAX)] ?? true);
            set => ViewState[nameof(EnableAJAX)] = value;
        }

        public bool HtmlDisabled
        {
            get { return ((bool?)ViewState[nameof(HtmlDisabled)]) ?? false; }
            set { ViewState[nameof(HtmlDisabled)] = value; }
        }

        private bool IsMultiValue
        {
            get => (bool?)ViewState[nameof(IsMultiValue)] == true;
            set => ViewState[nameof(IsMultiValue)] = value;
        }

        public bool TranslateDisabled
        {
            get { return ((bool?)ViewState[nameof(TranslateDisabled)]) ?? false; }
            set { ViewState[nameof(TranslateDisabled)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonScript.ContentKey = GetType().FullName;

            EditorFieldText.TranslationRequested += EditorField_TranslationRequested;
            EditorFieldHtml.TranslationRequested += EditorField_TranslationRequested;
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdatePanel.ChildrenAsTriggers = EnableAJAX;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(SectionMdHtml),
                "init_" + ClientID,
                $"sectionMdHtml.init({{" +
                $"toggleSelector:'#{IsBodyMarkdown.ClientID}'," +
                $"fieldTextSelector:'#{EditorFieldText.ClientID}'," +
                $"fieldHtmlSelector:'#{EditorFieldHtml.ClientID}'," +
                $"}});", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void EditorField_TranslationRequested(object sender, EditorTranslation.RequestEventArgs args)
        {
            var field = (Field)((Control)sender).NamingContainer;
            OnTranslationRequested(field.InputContent, args);
        }

        #endregion

        public void SelectEditor(ContentSectionDefault editor)
        {
            switch (editor)
            {
                case ContentSectionDefault.BodyText:
                    IsBodyMarkdown.Checked = true;
                    break;
                case ContentSectionDefault.BodyHtml:
                    IsBodyMarkdown.Checked = false;
                    break;
                default:
                    throw ApplicationError.Create("Unexpected ID: {0}", editor);
            }
        }

        public override void SetOptions(AssetContentSection options)
        {
            if (options is AssetContentSection.MarkdownAndHtml mdHtml)
            {
                IsMultiValue = mdHtml.IsMultiValue;

                IsBodyMarkdown.Checked = mdHtml.HtmlValue == null || mdHtml.HtmlValue.IsEmpty;

                EditorFieldText.SetOptions(new AssetContentSection.Markdown("md")
                {
                    Label = mdHtml.MarkdownLabel,
                    Description = mdHtml.MarkdownDescription,
                    IsRequired = mdHtml.IsRequired,
                    AllowUpload = mdHtml.AllowUpload,
                    UploadFolderPath = mdHtml.UploadFolderPath,
                    UploadStrategy = mdHtml.UploadStrategy,
                    Value = mdHtml.MarkdownValue
                });

                EditorFieldHtml.SetOptions(new AssetContentSection.Html("html")
                {
                    Label = mdHtml.HtmlLabel,
                    Description = mdHtml.HtmlDescription,
                    IsRequired = mdHtml.IsRequired,
                    AllowUpload = mdHtml.AllowUpload,
                    UploadFolderPath = mdHtml.UploadFolderPath,
                    UploadStrategy = mdHtml.UploadStrategy,
                    Value = mdHtml.HtmlValue
                });

                if (HtmlDisabled)
                {
                    BodyToggleWrapper.Visible = false;
                    EditorFieldHtml.Visible = false;
                }

                if (TranslateDisabled)
                {
                    EditorFieldText.TranslateDisabled = TranslateDisabled;
                    EditorFieldHtml.TranslateDisabled = TranslateDisabled;
                }
            }
            else
            {
                throw new NotImplementedException("Section type: " + options.GetType().FullName);
            }
        }

        public override void SetValidationGroup(string groupName)
        {
            EditorFieldText.ValidationGroup = groupName;
            EditorFieldHtml.ValidationGroup = groupName;
        }

        public Field GetField(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (string.Equals(id, ContentSectionDefault.BodyText.GetName(), StringComparison.OrdinalIgnoreCase))
                return EditorFieldText;
            else if (string.Equals(id, ContentSectionDefault.BodyHtml.GetName(), StringComparison.OrdinalIgnoreCase))
                return EditorFieldHtml;
            else
                throw ApplicationError.Create("Unexpected ID: {0}", id);
        }

        public override MultilingualString GetValue() => throw new NotImplementedException();

        public override MultilingualString GetValue(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (string.Equals(id, ContentSectionDefault.BodyText.GetName(), StringComparison.OrdinalIgnoreCase))
                return IsMultiValue || IsBodyMarkdown.Checked ? EditorFieldText.Translation : null;

            if (string.Equals(id, ContentSectionDefault.BodyHtml.GetName(), StringComparison.OrdinalIgnoreCase))
            {
                var translation = IsMultiValue || !IsBodyMarkdown.Checked ? EditorFieldHtml.Translation : null;

                if (translation != null)
                    foreach (var lang in translation.Languages)
                    {
                        var value = translation[lang];
                        if (value != null && (value == "<br>" || value == "<p></p>" || value == "<p><br></p>"))
                            translation[lang] = null;
                    }

                return translation;
            }

            throw ApplicationError.Create("Unexpected ID: {0}", id);
        }

        public override IEnumerable<MultilingualString> GetValues()
        {
            yield return GetValue(ContentSectionDefault.BodyText.GetName());
            yield return GetValue(ContentSectionDefault.BodyHtml.GetName());
        }

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        public override void OpenTab(string id) { }

        public override void SetLanguage(string lang)
        {
            EditorFieldText.InputLanguage = lang;
            EditorFieldHtml.InputLanguage = lang;
        }
    }
}