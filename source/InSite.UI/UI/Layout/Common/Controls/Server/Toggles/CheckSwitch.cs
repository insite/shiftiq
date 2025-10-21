using System.Web.UI;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class CheckSwitch : BaseCheck
    {
        public ToggleSwitchRenderMode RenderMode
        {
            get => (ToggleSwitchRenderMode)(ViewState[nameof(RenderMode)] ?? ToggleSwitchRenderMode.Stacked);
            set => ViewState[nameof(RenderMode)] = value;
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            if (RenderMode == ToggleSwitchRenderMode.Inline)
                RenderBlock(writer, "form-switch form-check-inline");
            else
                RenderBlock(writer, "form-switch");
        }

        protected override void AddInputAttributes(HtmlTextWriter writer)
        {
            base.AddInputAttributes(writer);

            writer.AddAttribute("role", "switch");
        }
    }
}