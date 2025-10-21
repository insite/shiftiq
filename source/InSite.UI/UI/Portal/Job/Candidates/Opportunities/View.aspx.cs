using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates
{
    public partial class View : PortalBasePage
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

                    if (_currentUser != null && !_currentUser.JobsApproved.HasValue &&
                        !(MembershipSearch.IsUserAssignedToDepartment(_currentUser.OrganizationIdentifier, _currentUser.UserIdentifier) ||
                          MembershipSearch.IsUserAssignedToRole(_currentUser.OrganizationIdentifier, _currentUser.UserIdentifier)))
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

        protected string SearchUrl => "/ui/portal/job/candidates/opportunities/search";
        protected Guid JobOpportunity => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;
        private Person _currentUser;
        private bool _isCurrentUserLoaded;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var temp = IsUserApproved;

            if (IsPostBack)
                return;

            Layout.Admin.PageHelper.AutoBindHeader(this);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            Approved.Visible = IsUserApproved;

            if (!IsUserApproved)
                StatusAlert.AddMessage(AlertType.Error, "You can apply when your profile is approved.");

            var jobOpportunity = TOpportunitySearch.Select(JobOpportunity);
            if (jobOpportunity == null)
            {
                HttpResponseHelper.Redirect(SearchUrl, true);
                return;
            }
            var title = $"{jobOpportunity.JobTitle} {jobOpportunity.JobLevel}";

            Page.Title = title;
            Apply.NavigateUrl = $"/ui/portal/job/candidates/opportunities/apply?id={jobOpportunity.OpportunityIdentifier}";

            Details.LoadData(jobOpportunity);

        }
    }
}