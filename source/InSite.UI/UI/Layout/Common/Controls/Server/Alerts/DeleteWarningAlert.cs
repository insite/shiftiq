using System;

namespace InSite.Common.Web.UI
{
    public class DeleteWarningAlert : System.Web.UI.WebControls.Literal
    {
        public string CssClass { get; set; }
        public string Name { get; set; }
        public bool NoSummary { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var entity = Shift.Common.Humanizer.LowerCase(Name);
            var aboutSummary = NoSummary ? "" : " Here is a summary of the data that will be removed if you proceed:";
            Text = $"<div class='{CssClass} d-flex alert alert-warning' role='alert'><i class='far fa-exclamation-triangle fs-xl me-3'></i><div><strong>Warning:</strong> This action <strong>cannot be undone</strong>. This will permanently remove the {entity} and its related data from all forms, queries, and reports.{aboutSummary}</div></div>";
        }
    }
}