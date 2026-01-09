using System;
using System.Web.UI.WebControls;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class IxCustomValidator : CustomValidator
    {
        public string ImageUrl { get; set; }

        public IxCustomValidator()
            => Display = ValidatorDisplay.Static;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (string.IsNullOrEmpty(Text))
                Text = $"<span class='text-danger' title='{ErrorMessage}'><i class='fas fa-circle'></i></span>";
        }
    }
}