using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Jobs.Applications.Forms
{
    public partial class Edit : AdminBasePage
    {
        private Guid ApplicationIdentifier => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            CancelButton.NavigateUrl = "/ui/admin/jobs/applications/search";

            LoadData();
        }

        private void LoadData()
        {
            var entity = TApplicationSearch.SelectJobApplication(ApplicationIdentifier, x => x.Opportunity, x => x.CandidateUser);

            if (entity == null || entity.Opportunity.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/jobs/applications/create");
                return;
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{entity.Opportunity.JobTitle} - {entity.Opportunity.EmployerGroupName}");

            Detail.SetInputValues(entity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = TApplicationSearch.SelectJobApplication(ApplicationIdentifier);

            Detail.GetInputValues(entity);

            entity.WhenModified = DateTime.UtcNow;

            TApplicationStore.Update(entity);

            StatusAlert.AddMessage(AlertType.Success, "Your changes have been successfully saved");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TApplicationStore.DeleteJobApplication(ApplicationIdentifier);

            HttpResponseHelper.Redirect("/ui/admin/jobs/applications/search");
        }
    }
}