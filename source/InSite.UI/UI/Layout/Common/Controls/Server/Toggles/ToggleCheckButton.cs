using System.Web.UI;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class ToggleCheckButton : BaseCheck
    {
        public ButtonStyle ButtonStyle
        {
            get => (ButtonStyle)(ViewState[nameof(ButtonStyle)] ?? ButtonStyle.OutlinePrimary);
            set => ViewState[nameof(ButtonStyle)] = value;
        }

        public ButtonSize Size
        {
            get => (ButtonSize)(ViewState[nameof(Size)] ?? ButtonSize.Small);
            set => ViewState[nameof(Size)] = value;
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            RenderInput(writer, false);

            AddToolTipAttributes(writer);

            RenderLabel(writer);
        }

        protected override string GetInputCssClass() => "btn-check";

        protected override string GetLabelCssClass()
        {
            return ControlHelper.MergeCssClasses(
                "btn",
                Size.GetContextualClass(),
                ButtonStyle.GetContextualClass(),
                CssClass
            );
        }
    }
}