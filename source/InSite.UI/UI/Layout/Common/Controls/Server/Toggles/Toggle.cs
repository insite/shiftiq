using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class Toggle : BaseCheck
    {
        #region Properties

        public string TextOn
        {
            get => (string)ViewState[nameof(TextOn)];
            set => ViewState[nameof(TextOn)] = value;
        }

        public string TextOff
        {
            get => (string)ViewState[nameof(TextOff)];
            set => ViewState[nameof(TextOff)] = value;
        }

        public ButtonStyle StyleOn
        {
            get => (ButtonStyle)(ViewState[nameof(StyleOn)] ?? ButtonStyle.Primary);
            set => ViewState[nameof(StyleOn)] = value;
        }

        public ButtonStyle StyleOff
        {
            get => (ButtonStyle)(ViewState[nameof(StyleOff)] ?? ButtonStyle.Default);
            set => ViewState[nameof(StyleOff)] = value;
        }

        public ToggleSize Size
        {
            get => (ToggleSize)(ViewState[nameof(Size)] ?? ToggleSize.Normal);
            set => ViewState[nameof(Size)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public Unit Height
        {
            get => (Unit)(ViewState[nameof(Height)] ?? Unit.Empty);
            set => ViewState[nameof(Height)] = value;
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Toggle),
                "init_" + ClientID,
                string.Format(" $('#{0}').bootstrapToggle(); ", ClientID),
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            RenderInput(writer, true);
        }

        protected override string GetInputCssClass() => string.Empty;

        protected override void AddInputAttributes(HtmlTextWriter writer)
        {
            base.AddInputAttributes(writer);

            if (TextOn.IsNotEmpty())
                writer.AddAttribute("data-on", TextOn);

            if (TextOff.IsNotEmpty())
                writer.AddAttribute("data-off", TextOff);

            if (StyleOn != ButtonStyle.Primary)
                writer.AddAttribute("data-onstyle", StyleOn.GetName().ToLower());

            if (StyleOff != ButtonStyle.Default)
                writer.AddAttribute("data-offstyle", StyleOff.GetName().ToLower());

            if (Size != ToggleSize.Normal)
                writer.AddAttribute("data-size", Size.GetName().ToLower());

            if (!Width.IsEmpty)
                writer.AddAttribute("data-width", Width.ToString());

            if (!Height.IsEmpty)
                writer.AddAttribute("data-height", Height.ToString());
        }

        #endregion
    }
}