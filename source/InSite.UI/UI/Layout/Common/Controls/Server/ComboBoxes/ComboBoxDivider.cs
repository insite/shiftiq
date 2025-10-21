using System.Web.UI;

namespace InSite.Common.Web.UI
{
    public sealed class ComboBoxDivider : ComboBoxItem
    {
        public override string Text { get => null; set { } }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute("data-divider", "true");

            writer.RenderBeginTag(HtmlTextWriterTag.Option);
            writer.RenderEndTag();
        }
    }
}