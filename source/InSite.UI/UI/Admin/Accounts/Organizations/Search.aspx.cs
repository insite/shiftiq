using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Accounts.Organizations
{
    public partial class Search : SearchPage<OrganizationFilter>
    {
        public override string EntityName => "Organization";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Organization", "/ui/admin/accounts/organizations/create", null, null));
        }
    }
}