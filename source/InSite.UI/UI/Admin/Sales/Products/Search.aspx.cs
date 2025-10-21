using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sales.Products
{
    public partial class Search : SearchPage<TProductFilter>
    {
        public override string EntityName => "Product";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Product", "/ui/admin/sales/products/create", null, null));
        }
    }
}