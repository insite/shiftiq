using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class SectionMdHtml : SectionBase
    {
        #region Events

        public class TranslationRequestEventArgs : EventArgs
        {
            #region Properties

            public Field.ContentTypeEnum ContentType { get; }

            public string FromLanguage { get; }

            public string[] ToLanguages { get; }

            #endregion

            #region Construction

            public TranslationRequestEventArgs(Field.ContentTypeEnum content, string fromLanguage, string[] toLanguages)
            {
                ContentType = content;
                FromLanguage = fromLanguage;
                ToLanguages = toLanguages;
            }

            #endregion
        }

        public delegate void TranslationRequestEventHandler(object sender, TranslationRequestEventArgs args);

        public event TranslationRequestEventHandler TranslationRequested;

        private void OnTranslationRequested(Field.ContentTypeEnum content, Field.TranslationRequestEventArgs fieldArgs) =>
            TranslationRequested?.Invoke(this, new TranslationRequestEventArgs(content, fieldArgs.FromLanguage, fieldArgs.ToLanguages));

        #endregion

        #region Properties

        private bool IsMultiValue
        {
            get => (bool?)ViewState[nameof(IsMultiValue)] == true;
            set => ViewState[nameof(IsMultiValue)] = value;
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

        private void EditorField_TranslationRequested(object sender, Field.TranslationRequestEventArgs args)
        {
            var field = (Field)sender;
            OnTranslationRequested(field.ContentType, args);
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
                    throw ApplicationError.Create("Unexpected pill ID: {0}", editor);
            }
        }

        public override void SetOptions(LayoutContentSection options)
        {
            if (options is LayoutContentSection.MarkdownAndHtml mdHtml)
            {
                IsMultiValue = mdHtml.IsMultiValue;

                IsBodyMarkdown.Checked = mdHtml.HtmlValue == null || mdHtml.HtmlValue.IsEmpty;

                EditorFieldText.SetOptions(new LayoutContentSection.Markdown("md")
                {
                    Label = mdHtml.MarkdownLabel,
                    Description = mdHtml.MarkdownDescription,
                    IsRequired = mdHtml.IsRequired,
                    AllowUpload = mdHtml.AllowUpload,
                    UploadFolderPath = mdHtml.UploadFolderPath,
                    Value = mdHtml.MarkdownValue
                });

                EditorFieldHtml.SetOptions(new LayoutContentSection.Html("html")
                {
                    Label = mdHtml.HtmlLabel,
                    Description = mdHtml.HtmlDescription,
                    IsRequired = mdHtml.IsRequired,
                    AllowUpload = mdHtml.AllowUpload,
                    UploadFolderPath = mdHtml.UploadFolderPath,
                    Value = mdHtml.HtmlValue
                });
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

            throw ApplicationError.Create("Unexpected pill ID: {0}", id);
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