using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.People.Forms
{
    public partial class Search : SearchPage<CmdsPersonFilter>, ICmdsUserControl
    {
        public override string EntityName => "Person";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SearchCriteria.ApplySecurityPermissions();

            if (IsPostBack)
                return;

            var permissionName = PermissionNames.Custom_CMDS_Workers;
            var canAdd = Access.Create
                || Identity.IsGranted(permissionName, PermissionOperation.Delete)
                || Identity.IsGranted(permissionName, PermissionOperation.Configure);

            BreadcrumbItem addNewItem = null;
            if (canAdd)
                addNewItem = new BreadcrumbItem("Add New Person", "/ui/cmds/admin/users/create", null, null);

            PageHelper.AutoBindHeader(this, addNewItem);
        }
    }
}