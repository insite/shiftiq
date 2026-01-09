using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Sales.Taxes
{
    public partial class Delete : AdminBasePage
    {
        private const string _searchUrl = "/ui/admin/sales/taxes/search";

        private Guid TaxIdentifier => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var tax = ServiceLocator.InvoiceSearch.GetTax(TaxIdentifier);
            if (tax == null || tax.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect($"/ui/admin/sales/taxes/search");
                return;
            }

            PageHelper.AutoBindHeader(this);

            TaxName.Text = tax.TaxName;
            CountryName.Text = ServiceLocator.CountrySearch.SelectByCode(tax.CountryCode)?.Name ?? tax.CountryCode;
            RegionName.Text = ServiceLocator.ProvinceSearch.Unabbreviate(tax.RegionCode);
            TaxPercent.Text = $"{tax.TaxRate:p2}";

            CancelButton.NavigateUrl = _searchUrl;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.InvoiceStore.DeleteTax(TaxIdentifier);

            HttpResponseHelper.Redirect(_searchUrl);
        }
    }
}