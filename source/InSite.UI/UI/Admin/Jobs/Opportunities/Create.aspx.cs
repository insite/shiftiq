using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Jobs.Opportunities.Controls;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Jobs.Opportunities.Forms
{
    public partial class Create : AdminBasePage
    {
        protected string OutlineUrl => "/ui/admin/jobs/opportunities/edit";
        protected string SearchUrl => "/ui/admin/jobs/opportunities/search";

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

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = SearchUrl;

            if (!IsPostBack)
                OnCreationTypeSelected();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var job = new TOpportunity();
            job.OrganizationIdentifier = Organization.Identifier;

            Details.GetInputValues(job);
            job.WhenCreated = DateTimeOffset.Now;

            TOpportunityStore.Insert(job);

            HttpResponseHelper.Redirect($"{OutlineUrl}?id={job.OpportunityIdentifier}");
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            SaveButton.Visible = true;

            MultiView.SetActiveView(DefaultView);
            Details.LoadDefault();
        }
    }
}