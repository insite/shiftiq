using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Portal.Contacts.People
{
    public partial class Search : SearchPage<PersonFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Identity.IsGranted("Portal/Contacts", PermissionOperation.Write))
                PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add new Contact", "/ui/portal/contacts/people/create", null, null));
            else
                PageHelper.AutoBindHeader(this);

            PortalMaster.RenderHelpContent(null);

            DashboardNavigation.Visible = PortalMaster.IsSalesReady;

            if (PortalMaster.IsSalesReady)
            {
                PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/profile");

                PortalMaster.HideBreadcrumbsOnly();

                if (Identity.IsAuthenticated)
                    OverrideHomeLink("/ui/portal/management/dashboard/home");
                else
                    OverrideHomeLink("/ui/portal/billing/catalog");
            }
            else
                PortalMaster.SidebarVisible(false);
        }
    }
}