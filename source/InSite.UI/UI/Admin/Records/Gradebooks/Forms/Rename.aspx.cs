using System;

using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class Rename : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier);

                if (gradebook == null || gradebook.OrganizationIdentifier != Organization.OrganizationIdentifier)
                    HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/search");

                PageHelper.AutoBindHeader(this, null, gradebook.GradebookTitle);

                GradebookTitle.Text = gradebook.GradebookTitle;

                CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=gradebook";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new RenameGradebook(GradebookIdentifier, GradebookTitle.Text));

            Course2Store.ClearCache(Organization.Identifier);

            HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=gradebook");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=gradebook"
                : null;
        }
    }
}
