using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Accounts.Permissions
{
    public partial class Search : SearchPage<TGroupActionFilter>
    {
        public override string EntityName => "Permission";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Permission", "/ui/admin/accounts/permissions/create", null, null));
        }
    }
}