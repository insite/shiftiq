using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(false)]
    public class LoadingPanel : Control, IHasText
    {
        #region Properties

        public string Text
        {
            get { return (string)ViewState[nameof(Text)]; }
            set { ViewState[nameof(Text)] = value; }
        }

        public string CssClass
        {
            get { return (string)ViewState[nameof(CssClass)]; }
            set { ViewState[nameof(CssClass)] = value; }
        }

        public bool VisibleOnLoad
        {
            get => (bool)(ViewState[nameof(VisibleOnLoad)] ?? false);
            set => ViewState[nameof(VisibleOnLoad)] = value;
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("loading-panel", CssClass));

            if (VisibleOnLoad)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:block !important;");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("<div><div>");

            writer.Write("<i class=\"fa fa-spinner fa-pulse fa-3x\"></i>");

            if (Text.IsNotEmpty())
            {
                writer.RenderBeginTag(HtmlTextWriterTag.P);
                writer.Write(Text);
                writer.RenderEndTag();
            }


            writer.Write("</div></div>");
            writer.RenderEndTag();
        }
    }
}