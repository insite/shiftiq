using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Portal.Contacts.People
{
    public partial class Search : SearchPage<PersonFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);
        }

        protected override void OnLoad(EventArgs e)
        {
            var hasContacts = Identity.IsGranted("Portal/Contacts");
            var hasUsers = Identity.IsGranted(PersonSearchGate.UserAdministrationResource);

            if (!PersonSearchGate.IsAccessAllowed(hasContacts, hasUsers))
                HttpResponseHelper.SendHttp403();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            SearchCriteria.ApplyRouteScope(Request.QueryString["scope"]);

            if (Identity.IsGranted("Portal/Contacts", DataAccess.Update))
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

        // The Downloads tab belongs to the design/users permission. The base makes it visible only
        // when the current result set has data; this additionally requires the permission, so a
        // Portal/Contacts-only caller never sees it.
        protected override void SetDownloadsVisiblity(bool visible)
        {
            var hasUsers = Identity.IsGranted(PersonSearchGate.UserAdministrationResource);

            base.SetDownloadsVisiblity(visible && hasUsers);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }
    }
}
