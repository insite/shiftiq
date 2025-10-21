using System;

namespace InSite.Common.Web.UI
{
    public class DeleteConfirmAlert : System.Web.UI.WebControls.Literal
    {
        public string CssClass { get; set; }
        public string Name { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var entity = Shift.Common.Humanizer.LowerCase(Name);
            Text = $"<div class='{CssClass} d-flex alert alert-danger' role='alert'><i class='far fa-stop-circle fs-xl me-3'></i><div><strong>Confirm:</strong> Are you absolutely certain you want to delete this {entity} and all its related data?</div></div>";
        }
    }
}