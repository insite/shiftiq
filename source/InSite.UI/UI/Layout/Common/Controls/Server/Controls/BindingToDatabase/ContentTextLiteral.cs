using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ContentTextLiteral : Literal
    {
        public string ContentLabel
        {
            get => (string)ViewState[nameof(ContentLabel)];
            set => ViewState[nameof(ContentLabel)] = value;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (ContentLabel == null)
                return;

            var label = LabelHelper.GetLabelContentText(ContentLabel);

            if (label.HasNoValue())
                writer.Write(ContentLabel);
            else
                writer.Write(label);
        }
    }
}