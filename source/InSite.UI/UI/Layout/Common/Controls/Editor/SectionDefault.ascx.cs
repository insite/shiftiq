using System;
using System.Collections.Generic;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class SectionDefault : SectionBase
    {
        public override void SetOptions(LayoutContentSection options) => EditorField.SetOptions(options);

        public override void SetValidationGroup(string groupName) => EditorField.ValidationGroup = groupName;

        public override MultilingualString GetValue()
        {
            var translation = EditorField.Translation;

            if (EditorField.ContentType == Field.ContentTypeEnum.Html)
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
        
        public override MultilingualString GetValue(string id) => throw new NotImplementedException();

        public override IEnumerable<MultilingualString> GetValues() => throw new NotImplementedException();

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        public override void OpenTab(string id) { }

        public override void SetLanguage(string lang) => EditorField.InputLanguage = lang;
    }
}