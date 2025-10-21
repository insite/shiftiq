using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Accounts.Departments
{
    public partial class Search : SearchPage<DepartmentFilter>
    {
        public override string EntityName => "Department";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsQueryStringValid(Request.QueryString, null, null, SearchAlert))
                DisableForm();

            else if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Department", "/ui/admin/accounts/departments/create", null, null));
        }
    }
}