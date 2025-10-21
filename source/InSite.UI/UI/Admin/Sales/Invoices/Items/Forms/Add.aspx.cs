using System;
using System.Web.UI;

using InSite.Application.Invoices.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Invoices;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Invoices.Items.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? InvoiceID => Guid.TryParse(Request["invoice"], out Guid result) ? result : (Guid?)null;

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
                var invoice = InvoiceID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value) : null;
                if (invoice == null || invoice.OrganizationIdentifier != Organization.OrganizationIdentifier)
                {
                    HttpResponseHelper.Redirect("/ui/admin/sales/invoices/search");
                    return;
                }

                var title = $"Invoice to {invoice.CustomerFullName}";

                PageHelper.AutoBindHeader(this, null, title);

                // Set Default Values

                CancelButton.NavigateUrl = $"/ui/admin/sales/invoices/outline?id={InvoiceID}";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var command = new AddInvoiceItem(
                InvoiceID.Value,
                new InvoiceItem
                {
                    Identifier = UniqueIdentifier.Create(),
                    Description = ItemDescription.Text,
                    Price = ItemPrice.ValueAsDecimal.Value,
                    Quantity = ItemQuantity.ValueAsInt.Value,
                    Product = ProductID.ValueAsGuid.Value
                }
            );

            ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/sales/invoices/outline?id={InvoiceID}");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={InvoiceID}"
                : null;
        }
    }
}