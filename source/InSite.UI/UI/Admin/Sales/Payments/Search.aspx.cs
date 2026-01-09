using System;

using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Sales.Payments
{
    public partial class Search : SearchPage<QPaymentFilter>
    {
        public override string EntityName => "Payment";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}