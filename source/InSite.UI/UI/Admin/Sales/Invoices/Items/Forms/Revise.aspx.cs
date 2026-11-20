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
    public partial class Revise : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? InvoiceID => Guid.TryParse(Request["invoice"], out Guid result) ? result : (Guid?)null;

        private Guid? ItemID => Guid.TryParse(Request["item"], out Guid result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var invoice = InvoiceID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value) : null;
            if (invoice == null || invoice.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            var invoiceItem = ItemID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoiceItem(InvoiceID.Value, ItemID.Value) : null;
            if (invoiceItem == null)
            {
                RedirectToOutline();
                return;
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"Invoice to {invoice.CustomerFullName} <span class='form-text'>Revise Invoice Item #{invoiceItem.ItemSequence}</span>");

            ItemDescription.Text = invoiceItem.ItemDescription;
            ItemPrice.ValueAsDecimal = invoiceItem.ItemPrice;
            ItemQuantity.ValueAsInt = invoiceItem.ItemQuantity;

            ProductID.EnsureDataBound();
            ProductID.ValueAsGuid = invoiceItem.ProductIdentifier;

            CancelButton.NavigateUrl = GetOutlineUrl();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
            RedirectToOutline();
        }

        private void Save()
        {
            var currentItem = ServiceLocator.InvoiceSearch.GetInvoiceItem(InvoiceID.Value, ItemID.Value);
            if (currentItem == null)
                return;

            var command = new ChangeInvoiceItem(
                InvoiceID.Value,
                new InvoiceItem
                {
                    Identifier = ItemID.Value,
                    Product = ProductID.ValueAsGuid.Value,
                    Quantity = ItemQuantity.ValueAsInt.Value,
                    Price = ItemPrice.ValueAsDecimal.Value,
                    Description = ItemDescription.Text,
                    TaxRate = currentItem.TaxRate,
                    TaxCountry = currentItem.TaxCountry,
                    TaxRegion = currentItem.TaxRegion
                }
            );

            ServiceLocator.SendCommand(command);
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect("/ui/admin/sales/invoices/search");

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl());

        private string GetOutlineUrl() =>
            $"/ui/admin/sales/invoices/outline?id={InvoiceID}";

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={InvoiceID}"
                : null;
        }
    }
}
