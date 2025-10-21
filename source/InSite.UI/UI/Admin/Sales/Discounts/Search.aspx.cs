using System;

using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sales.Discounts
{
    public partial class Search : SearchPage<TDiscountFilter>
    {
        public override string EntityName => "Discount";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Discount", "/ui/admin/sales/discounts/create", null, null));
        }
    }
}