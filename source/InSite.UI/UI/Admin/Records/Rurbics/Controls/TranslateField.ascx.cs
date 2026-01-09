using System;
using System.Web.UI.HtmlControls;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Records.Rurbics.Controls
{
    public partial class TranslateField : BaseUserControl
    {
        public string FieldName
        {
            get => (string)ViewState[nameof(FieldName)];
            set => ViewState[nameof(FieldName)] = value;
        }

        public string TextOriginal
        {
            get => (string)ViewState[nameof(TextOriginal)];
            set => ViewState[nameof(TextOriginal)] = value;
        }

        public string TextTranslated
        {
            get => (string)ViewState[nameof(TextTranslated)];
            set => ViewState[nameof(TextTranslated)] = value;
        }

        public bool AllowTranslate
        {
            get => TranslateButton.Enabled;
            set => TranslateButton.Enabled = value;
        }

        public string OnButtonClientClick
        {
            get => TranslateButton.OnClientClick;
            set => TranslateButton.OnClientClick = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = CommonScript.ContentKey = typeof(TranslateField).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            FieldNameOriginal.Text = FieldName;
            FieldNameTranslated.Text = FieldName;

            SetField(OriginalValue, TextOriginal);
            SetField(TranslatedValue, TextTranslated);

            base.OnPreRender(e);

            void SetField(HtmlGenericControl output, string value)
            {
                if (value.HasValue())
                    output.InnerText = value;
                else
                    output.InnerHtml = "&nbsp;";
            }
        }
    }
}
