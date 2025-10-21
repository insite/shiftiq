using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Sales.Orders
{
    public partial class Search : SearchPage<TOrderFilter>
    {
        public override string EntityName => "Order";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}