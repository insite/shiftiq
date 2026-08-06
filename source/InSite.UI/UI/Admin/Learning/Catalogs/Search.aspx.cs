using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Learning.Catalogs
{
    public partial class Search : SearchPage<TCatalogFilter>
    {
        public override string EntityName => "Catalogue";

        public const string NavigateUrl = "/ui/admin/learning/catalogs/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Catalogue", "/ui/admin/learning/catalogs/create", null, null));
        }
    }
}