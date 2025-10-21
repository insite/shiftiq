using System;
using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Sales.Taxes
{
    public partial class Create : AdminBasePage
    {
        private string FinderRelativePath => "/ui/admin/sales/taxes/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                CancelLink.NavigateUrl = FinderRelativePath;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var tax = new TTax();
            tax.TaxIdentifier = UniqueIdentifier.Create();
            tax.OrganizationIdentifier = Organization.OrganizationIdentifier;
            tax.CountryCode = "CA";

            Detail.GetInputValues(tax);

            ServiceLocator.InvoiceStore.InsertTax(tax);

            HttpResponseHelper.Redirect($"/ui/admin/sales/taxes/edit?id={tax.TaxIdentifier}&status=saved");
        }
    }
}