using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Foundations;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class UserNavMenu : BaseUserControl
    {
        public bool SimpleMode { get; set; } = false;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Visible = User != null;

            if (!Visible)
                return;

            BindOrganizationLinks();
            BindUserLinks();
            ApplySimpleMode();
        }

        internal void ApplySimpleMode()
        {
            if (!SimpleMode)
                return;

            MyDashboardItem.Visible = false;
            MyProfileItem.Visible = false;
            SelectEnvironmentItem.Visible = false;
            SelectOrganizationItem.Visible = false;
            MyBackgrounds.Visible = false;

            SignOutItem.Visible = Request.IsAuthenticated;
        }

        private void BindOrganizationLinks()
        {
            var identity = CurrentSessionState.Identity;

            // You can select another environment only if you are an administrator in the current organization account.

            SelectEnvironmentItem.Visible = identity.Person.IsAdministrator;

            // You can select another organization only if you are a multi-organization user.

            var isMultiOrganization = identity.Persons.Count(x => x.IsAdministrator || x.IsLearner) > 1;
            if (isMultiOrganization)
            {
                SelectOrganizationLink.HRef = "/ui/portal/security/organizations?auto-redirect=0";
                SelectOrganizationItem.Visible = true;
            }
        }

        private void BindUserLinks()
        {
            MyDashboardLink.HRef = RelativeUrl.PortalHomeUrl;
            MyDashboardLink.InnerText = GetDisplayText("My Dashboard");
            MyDashboardItem.Visible = Organization.Toolkits.Portal.ShowMyDashboard;

            SignOutItem.Visible = Request.IsAuthenticated;
            SignOutLink.HRef = InSite.UI.Lobby.SignOut.GetUrl();

            UserName.InnerHtml = "<i class='fa-regular fa-user me-1'></i>" + User.FirstName;
        }
    }
}