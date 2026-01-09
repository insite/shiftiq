using System;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sites.Pages
{
    public partial class Search : SearchPage<QPageFilter>
    {
        public override string EntityName => "Page";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string type =  Request.QueryString["type"];

            PageHelper.AutoBindHeader(this, new BreadcrumbItem($"Add New {type}", "/ui/admin/sites/pages/create", null, null));
        }
    }
}