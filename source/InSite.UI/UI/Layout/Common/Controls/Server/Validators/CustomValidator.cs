using System;
using System.Web.UI.WebControls;

namespace InSite.Common.Web.UI
{
    public class CustomValidator : System.Web.UI.WebControls.CustomValidator
    {
        #region Construction

        public CustomValidator() { Display = ValidatorDisplay.Static; }

        #endregion

        #region Rendering

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (string.IsNullOrEmpty(Text))
            {
                Text = string.Format(
                    "<span class='text-danger' title='{0}'><i class='fas fa-circle'></i></span>",
                    !string.IsNullOrEmpty(ToolTip) ? ToolTip : ErrorMessage);
            }
        }

        #endregion
    }
}