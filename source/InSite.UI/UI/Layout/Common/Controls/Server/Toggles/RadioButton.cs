using System.Web.UI;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class RadioButton : BaseRadio
    {
        public CheckBoxRenderMode RenderMode
        {
            get => (CheckBoxRenderMode)(ViewState[nameof(RenderMode)] ?? CheckBoxRenderMode.Stacked);
            set => ViewState[nameof(RenderMode)] = value;
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            if (RenderMode == CheckBoxRenderMode.Input)
                RenderInput(writer, true);
            else if (RenderMode == CheckBoxRenderMode.Inline)
                RenderBlock(writer, "form-check-inline");
            else
                RenderBlock(writer, null);
        }
    }
}