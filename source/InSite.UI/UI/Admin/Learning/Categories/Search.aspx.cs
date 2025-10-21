using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Learning.Categories
{
    public partial class Search : SearchPage<TCollectionItemFilter>
    {
        public override string EntityName => "Category";

        public const string NavigateUrl = "/ui/admin/learning/categories/search";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Category", Create.NavigateUrl, null, null));
        }
    }
}
