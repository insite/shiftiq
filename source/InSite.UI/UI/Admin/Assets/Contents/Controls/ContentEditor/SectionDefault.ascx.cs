using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assets.Contents.Controls.ContentEditor
{
    public partial class SectionDefault : SectionBase
    {

        public bool TranslateDisabled
        {
            get { return ((bool?)ViewState[nameof(TranslateDisabled)]) ?? false; }
            set { ViewState[nameof(TranslateDisabled)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EditorField.TranslationRequested += EditorField_TranslationRequested;
        }

        private void EditorField_TranslationRequested(object sender, EditorTranslation.RequestEventArgs args)
        {
            var field = (Field)((Control)sender).NamingContainer;
            OnTranslationRequested(field.InputContent, args);
        }

        public event TranslationRequestEventHandler TranslationRequested;

        private void OnTranslationRequested(Field.ContentType content, EditorTranslation.RequestEventArgs args) =>
            TranslationRequested?.Invoke(this, new TranslationRequestEventArgs(content, args.FromLanguage, args.ToLanguages));

        public override void SetOptions(AssetContentSection options)
        {
            if (TranslateDisabled)
                EditorField.TranslateDisabled = TranslateDisabled;

            EditorField.SetOptions(options);
        }

        public override void SetValidationGroup(string groupName) => EditorField.ValidationGroup = groupName;

        public override MultilingualString GetValue()
        {
            var translation = EditorField.Translation;

            if (EditorField.InputContent == Field.ContentType.Html)
            {
                foreach (var lang in translation.Languages)
                {
                    var value = translation[lang];
                    if (value != null && (value == "<br>" || value == "<p></p>" || value == "<p><br></p>"))
                        translation[lang] = null;
                }
            }

            return translation;
        }

        public override MultilingualString GetValue(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return EditorField.Translation;
        }

        public Field GetField(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return EditorField;
        }

        //public override IEnumerable<MultilingualString> GetValues() => throw new NotImplementedException();

        public override IEnumerable<MultilingualString> GetValues()
        {
            yield return GetValue(ContentSectionDefault.BodyText.GetName());
        }

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        public override void OpenTab(string id) { }

        public override void SetLanguage(string lang) => EditorField.InputLanguage = lang;
    }
}