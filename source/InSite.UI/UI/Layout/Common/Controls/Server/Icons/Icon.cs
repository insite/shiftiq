using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class Icon : WebControl
    {
        #region Properties

        public string Name
        {
            get => (string)ViewState[nameof(Name)];
            set => ViewState[nameof(Name)] = value;
        }

        public IconType Type
        {
            get => (IconType?)ViewState[nameof(Type)] ?? IconType.Solid;
            set => ViewState[nameof(Type)] = value;
        }

        public Indicator Color
        {
            get => (Indicator)(ViewState[nameof(Indicator)] ?? Indicator.None);
            set => ViewState[nameof(Indicator)] = value;
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            RenderIcon(writer, false);
        }

        protected void RenderIcon(HtmlTextWriter writer, bool isChild)
        {
            if (!isChild)
                AddAttributesToRender(writer);

            var cssClass = Type.GetContextualClass();

            if (Name.IsNotEmpty())
                cssClass += " fa-" + Name;

            if (Color != Indicator.None)
                cssClass += " text-" + Color.GetContextualClass();

            if (!isChild && CssClass.IsNotEmpty())
                cssClass += " " + CssClass;

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();
        }

        #endregion
    }
}