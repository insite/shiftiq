using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities
{
    public partial class View : PortalBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {

        #region Properties

        protected Person CurrentUser
        {
            get
            {
                if (!_isCurrentUserLoaded)
                {
                    _currentUser = PersonSearch.Select(
                        CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                        CurrentSessionState.Identity.User.UserIdentifier
                    );

                    if (_currentUser != null && !MembershipSearch.IsUserAssignedToDepartment(_currentUser.OrganizationIdentifier, _currentUser.UserIdentifier))
                        _currentUser = null;

                    _isCurrentUserLoaded = true;
                }

                return _currentUser;
            }
        }

        public bool IsUserApproved =>
            CurrentUser != null && CurrentUser.JobsApproved.HasValue && CurrentUser.UserAccessGranted.HasValue;

        #endregion

        #region Fields

        protected string SearchUrl => "/ui/portal/job/employers/opportunities/search";
        protected Guid JobOpportunity => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;
        private Person _currentUser;
        private bool _isCurrentUserLoaded;

        #endregion

        private void CanEdit(Guid? oppprtunityEmployerId, Guid? personEmployerId)
        {
            if (oppprtunityEmployerId.HasValue && personEmployerId.HasValue)
                CandidatesGrid.Visible = (oppprtunityEmployerId == personEmployerId.Value);
            else
                CandidatesGrid.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Layout.Admin.PageHelper.AutoBindHeader(this);

            if (!IsPostBack)
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
            var title = $"{jobOpportunity.JobTitle} {jobOpportunity.JobLevel}";

            Page.Title = title;

            var CurrentUser = PersonSearch.Select(Organization.OrganizationIdentifier, CurrentSessionState.Identity.User.UserIdentifier);
            CanEdit(jobOpportunity.EmployerGroupIdentifier, CurrentUser.EmployerGroupIdentifier);

            EditLinkPanel.Visible = CandidatesGrid.Visible;

            CandidatesGrid.LoadData(jobOpportunity);

            Details.LoadData(jobOpportunity);

            BackLink.NavigateUrl = GetReaderUrl();
            EditLink.NavigateUrl = GetRedirectUrl();
        }

        #region Methods (redirect)

        protected string GetRedirectUrl()
        {
            return new ReturnUrl()
                .GetRedirectUrl($"/ui/portal/job/employers/opportunities/edit?id={JobOpportunity}");
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
            return parent.Name.EndsWith("/edit")
                ? $"id={JobOpportunity}"
                : GetParentLinkParameters(parent, null);
        }

        #endregion
    }
}