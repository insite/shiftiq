using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Jobs.Opportunities
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        protected string HomeUrl => "/ui/admin/jobs/home";
        protected string OutlineUrl => "/ui/admin/jobs/opportunities/edit";
        protected string SearchUrl => "/ui/admin/jobs/opportunities/search";

        protected string StatusMessage => "Opportunity cannot be delete since it already has applicants.";

        protected Guid? JobOpportunity => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;

        private TOpportunity _opportunity;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"id={JobOpportunity}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCheck.AutoPostBack = true;
            DeleteCheck.CheckedChanged += (x, y) => 
            { DeleteButton.Enabled = DeleteCheck.Checked && !HaveApplications(JobOpportunity); };
            DeleteButton.Click += OnConfirmed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete))
                NavigateToSearch();

            if (!ValidateOpportunity())
                NavigateToSearch();

            BindNavigation();
            BindOpportunity();
            BindImpact();
        }

        private void BindNavigation()
        {
            CancelButton.NavigateUrl = $"{OutlineUrl}?id={JobOpportunity.Value}";

            PageHelper.AutoBindHeader(this, null, _opportunity.JobTitle);
        }

        private void BindOpportunity()
        {
            OpportunityDetails.BindOpportunity(_opportunity);
        }

        private void BindImpact()
        {
            ApplicationsCount.Text = _opportunity.Applications != null ? _opportunity.Applications.Count.ToString() : "0";
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (JobOpportunity.HasValue)
                TOpportunityStore.Delete(JobOpportunity.Value);

            NavigateToSearch();
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private bool HaveApplications(Guid? jobOpportunity)
        {
            if (_opportunity.Applications != null & _opportunity.Applications.Count > 0)
            {
                Status.AddMessage(AlertType.Error, StatusMessage);
                return true;
            }
            return false;
        }

        private bool ValidateOpportunity()
        {
            _opportunity = JobOpportunity.HasValue ? TOpportunitySearch.Select(JobOpportunity.Value,x=>x.Applications) : null;
            if (_opportunity != null &&
                _opportunity.Applications != null &&
                _opportunity.Applications.Count > 0)
            {
                Status.AddMessage(AlertType.Error, StatusMessage);
            }
            return _opportunity != null && _opportunity.OrganizationIdentifier == Organization.OrganizationIdentifier;
        }
    }
}