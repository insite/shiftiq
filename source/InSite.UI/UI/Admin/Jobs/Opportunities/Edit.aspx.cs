using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Jobs.Opportunities.Forms
{
    public partial class Edit : AdminBasePage
    {
        protected string OutlineUrl => "/ui/admin/jobs/opportunities/edit";
        protected string SearchUrl => "/ui/admin/jobs/opportunities/search";
        protected string DeleteUrl => "/ui/admin/jobs/opportunities/delete";
        protected Guid JobOpportunity => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            if (IsPostBack)
                return;

            CancelButton.NavigateUrl = SearchUrl;

            LoadData();
        }

        private void LoadData()
        {
            var jobOpportunity = TOpportunitySearch.Select(JobOpportunity);
            if (jobOpportunity == null)
            {
                HttpResponseHelper.Redirect(SearchUrl, true);
                return;
            }

            PageHelper.AutoBindHeader(this, qualifier: $"{jobOpportunity.JobTitle} {jobOpportunity.JobLevel}");

            DeleteButton.NavigateUrl = $"{DeleteUrl}?id={JobOpportunity}";

            OnCreationTypeSelected(jobOpportunity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var job = TOpportunitySearch.Select(JobOpportunity);

            Details.GetInputValues(job);

            job.WhenCreated = DateTimeOffset.Now;

            TOpportunityStore.Update(job);

            HttpResponseHelper.Redirect($"{OutlineUrl}?id={job.OpportunityIdentifier}");
        }

        private void OnCreationTypeSelected(TOpportunity jobOpportunity = null)
        {
            MultiView.SetActiveView(DefaultView);
            Details.SetInputValues(GetJobOpportunity());

            TOpportunity GetJobOpportunity()
            {
                return jobOpportunity ?? (jobOpportunity = TOpportunitySearch.Select(JobOpportunity));
            }
        }
    }
}