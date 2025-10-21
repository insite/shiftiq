using System;
using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Invoices.Forms
{
    public partial class ChangeInvoiceDetails : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? InvoiceID => Guid.TryParse(Request["invoice"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/sales/invoices/search";

            InvoiceStatus.AutoPostBack = true;
            InvoiceStatus.ValueChanged += InvoiceStatus_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var invoice = InvoiceID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value) : null;
                if (invoice == null)
                    RedirectToSearch();

                var title = $"Invoice {invoice.InvoiceNumber} to {invoice.CustomerFullName}";

                PageHelper.AutoBindHeader(this, null, title);
                SetInputValues(invoice);

                CancelButton.NavigateUrl = GetOutlineUrl(InvoiceID.Value);
            }
        }

        public void SetInputValues(VInvoice invoice)
        {
            InvoiceStatus.EnsureDataBound();

            InvoicePaidOn.Value = invoice.InvoicePaid;
            InvoiceStatus.Value = invoice.InvoiceStatus;

            ShowInvoicePaidOnRequired();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value);

            if (invoice.InvoicePaid != InvoicePaidOn.Value)
            {
                var command = new ChangeInvoicePaidDate(InvoiceID.Value, InvoicePaidOn.Value);
                ServiceLocator.SendCommand(command);
            }

            if (invoice.InvoiceStatus != InvoiceStatus.Value)
            {
                var command = new ChangeInvoiceStatus(InvoiceID.Value, InvoiceStatus.Value);
                ServiceLocator.SendCommand(command);
            }

            RedirectToOutline();
        }

        private void InvoiceStatus_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            ShowInvoicePaidOnRequired();
        }

        private void ShowInvoicePaidOnRequired()
        {
            InvoicePaidOnRequired.Visible = string.Equals(InvoiceStatus.Value, InvoiceStatusType.Paid.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl(InvoiceID.Value), true);

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/sales/invoices/search", true);

        private string GetOutlineUrl(Guid invoiceID) =>
            $"/ui/admin/sales/invoices/outline?id={invoiceID}";

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={InvoiceID.Value}"
                : null;
        }
    }
}