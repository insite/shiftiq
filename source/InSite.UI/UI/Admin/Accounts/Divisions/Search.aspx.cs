using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Accounts.Divisions
{
    public partial class Search : SearchPage<DivisionFilter>
    {
        public override string EntityName => "Division";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsQueryStringValid(Request.QueryString, null, null, SearchAlert))
                return;

            if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Division", "/ui/admin/accounts/divisions/create", null, null));
        }
    }
}