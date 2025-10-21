using System;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sites.Sites
{
    public partial class Search : SearchPage<QSiteFilter>
    {
        public override string EntityName => "Site";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Site", "/ui/admin/sites/create", null, null));
        }
    }
}