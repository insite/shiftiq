using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Assets.Collections
{
    public partial class Search : SearchPage<TCollectionFilter>
    {
        public override string EntityName => "Collection";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Collection", "/ui/admin/assets/collections/create", null, null));
        }
    }
}