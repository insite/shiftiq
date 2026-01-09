using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ButtonSpacer : Control
    {
        public string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (CssClass.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:inline-block; margin:5px; color:#F5F5F5;");

            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            writer.Write("|");

            writer.RenderEndTag();
        }
    }
}