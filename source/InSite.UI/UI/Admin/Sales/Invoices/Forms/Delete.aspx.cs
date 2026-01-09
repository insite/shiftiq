using System;

using InSite.Application.Invoices.Write;
using InSite.Application.Payments.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

using InvoiceStatusEnum = Shift.Constant.InvoiceStatus;

namespace InSite.UI.Admin.Invoices.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? InvoiceID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = "/ui/admin/sales/invoices/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var invoice = InvoiceID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value) : null;
            if (invoice == null
                || invoice.OrganizationIdentifier != Organization.OrganizationIdentifier
                || !string.Equals(invoice.InvoiceStatus, InvoiceStatusEnum.Drafted.ToString(), StringComparison.OrdinalIgnoreCase)
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/sales/invoices/search");
                return;
            }

            var paymentCount = ServiceLocator.PaymentSearch.CountPayments(new QPaymentFilter { InvoiceIdentifier = InvoiceID.Value });

            PageHelper.AutoBindHeader(this, null, $"to {invoice.CustomerFullName}");

            CustomerName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={invoice.CustomerIdentifier}\">{invoice.CustomerFullName}</a>";
            InvoiceStatus.Text = invoice.InvoiceStatus;
            InvoiceDrafted.Text = GetLocalTime(invoice.InvoiceDrafted);
            InvoiceSubmitted.Text = GetLocalTime(invoice.InvoiceSubmitted);
            InvoicePaid.Text = GetLocalTime(invoice.InvoicePaid);

            ItemCount.Text = $"{invoice.ItemCount:n0}";
            PaymentCount.Text = $"{paymentCount:n0}";

            var backUrl = $"/ui/admin/sales/invoices/outline?id={InvoiceID}";

            CancelButton.NavigateUrl = backUrl;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteInvoice((Guid)InvoiceID));

            HttpResponseHelper.Redirect("/ui/admin/sales/invoices/search");
        }

        private string GetLocalTime(DateTimeOffset? item)
        {
            return item.Format(User.TimeZone, nullValue: "N/A", isHtml: true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={InvoiceID}"
                : null;
        }
    }
}