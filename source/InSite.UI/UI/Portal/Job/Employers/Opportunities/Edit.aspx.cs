using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities
{
    public partial class Edit : PortalBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        protected Guid JobOpportunity => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var person = PersonSearch.Select(Organization.Identifier, User.Identifier, x => x.EmployerGroup);
            var employer = person.EmployerGroup;

            if (employer == null)
            {
                StatusAlert.AddMessage(AlertType.Error, "No employer is selected for your account. Please contact your administrator to resolve this.");
                return;
            }

            var jobOpportunity = TOpportunitySearch.Select(JobOpportunity);
            if (jobOpportunity == null)
                HttpResponseHelper.Redirect("/");

            CandidatesGrid.LoadData(jobOpportunity);

            QGroup jobEmployer = null;
            if (jobOpportunity.EmployerGroupIdentifier.HasValue)
                jobEmployer = ServiceLocator.GroupSearch.GetGroup(jobOpportunity.EmployerGroupIdentifier.Value, x => x.Parent);

            OnCreationTypeSelected(jobOpportunity, jobEmployer);

            ViewLink.NavigateUrl = GetRedirectUrl();
            CancelButton.NavigateUrl = GetReaderUrl();

            PageHelper.AutoBindHeader(Page);
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected(TOpportunity jobOpportunity = null, QGroup jobEmployer = null)
        {
            Details.SetInputValues(jobOpportunity);
            MultiView.SetActiveView(DefaultView);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var job = TOpportunitySearch.Select(JobOpportunity);
            Details.GetInputValues(job);
            job.WhenModified = DateTimeOffset.Now;
            TOpportunityStore.Update(job);
            CandidatesGrid.LoadData(job);

            StatusAlert.AddMessage(AlertType.Success, "Your changes have been successfully saved");
        }

        #region Methods (redirect)

        protected string GetRedirectUrl()
        {
            return new ReturnUrl()
                .GetRedirectUrl($"/ui/portal/job/employers/opportunities/view?id={JobOpportunity}");
        }

        private string GetReaderUrl()
        {
            return new ReturnUrl().GetReturnUrl()
                ?? $"/ui/portal/job/employers/opportunities/search";
        }

        #endregion


        #region IHasParentLinkParameters

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/view")
                ? $"id={JobOpportunity}"
                : GetParentLinkParameters(parent, null);
        }

        #endregion
    }
}