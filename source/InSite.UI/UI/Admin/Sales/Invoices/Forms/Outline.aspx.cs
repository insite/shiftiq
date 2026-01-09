using System;
using System.Collections.Generic;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Payments.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Billing.Utilities;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;
using InvoiceStatusEnum = Shift.Constant.InvoiceStatus;

namespace InSite.Admin.Invoices.Forms
{
    public partial class Outline : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? InvoiceID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;
        private Guid? CustomerID => Guid.TryParse(Request["customer"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InvoiceItemGrid.Refreshed += InvoiceItemGrid_Refreshed;

            PrintInvoiceEventButton.Click += PrintInvoiceEventButton_Click;
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
            var invoice = InvoiceID.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceID.Value, x => x.Payments) : null;
            if (invoice == null || invoice.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/sales/invoices/search");
                return;
            }

            if (!CustomerID.HasValue)
            {
                PageHelper.AutoBindHeader(this, qualifier: $"Invoice to {invoice.CustomerFullName}");
            }
            else
            {
                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                        new BreadcrumbItem("Contacts", "/ui/admin/contacts/home"),
                        new BreadcrumbItem("People", null),
                        new BreadcrumbItem("Search", "/ui/admin/contacts/people/search"),
                        new BreadcrumbItem("Edit", $"/ui/admin/contacts/people/edit?contact={CustomerID.Value}"),
                        new BreadcrumbItem("Report", $"/ui/admin/contacts/people/report?contact=={CustomerID.Value}"),
                        new BreadcrumbItem("Invoice", null, null, "active"),
                }, null, qualifier: $"Invoice to {invoice.CustomerFullName}");
            }

            CancelButton.NavigateUrl = GetReturnUrl();
            NavigationSection.Visible = CancelButton.NavigateUrl.HasValue();

            EmployerContactName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={invoice.CustomerIdentifier}\">{invoice.CustomerFullName}</a>";
            InvoiceIdentifier.Text = invoice.InvoiceIdentifier.ToString();
            InvoiceNumber.Text = invoice.InvoiceNumber.HasValue ? invoice.InvoiceNumber.Value.ToString() : "None";
            InvoiceStatus.Text = GetInvoiceStatus(invoice.InvoiceStatus);
            InvoiceDrafted.Text = GetLocalTime(invoice.InvoiceDrafted);
            InvoiceSubmitted.Text = GetLocalTime(invoice.InvoiceSubmitted);
            InvoicePaid.Text = GetLocalTime(invoice.InvoicePaid);

            BusinessCustomerName.Text = invoice.BusinessCustomerGroupIdentifier != null
                ? $"<a href=\"/ui/admin/contacts/groups/edit?contact={invoice.BusinessCustomerGroupIdentifier}\">{invoice.BusinessCustomerName}</a>"
                : "None";

            ChangeCustomer.NavigateUrl = $"/ui/admin/sales/invoices/change-customer?invoice={InvoiceID}";
            ChangeIssuedToCompany.NavigateUrl = ChangeCustomer.NavigateUrl;
            UpdateInvoiceStatus.NavigateUrl = $"/ui/admin/sales/invoices/change-invoice-details?invoice={InvoiceID}";
            UpdateInvoicePaidDate.NavigateUrl = $"/ui/admin/sales/invoices/change-invoice-details?invoice={InvoiceID}";

            InvoiceItemGrid.LoadData(InvoiceID.Value, !HasTransactionCode(invoice.Payments));
            SetupItemSection();

            PaymentGrid.LoadData(InvoiceID.Value);
            PaymentSection.Visible = PaymentGrid.RowCount > 0;

            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(InvoiceID.Value, $"/ui/admin/sales/invoices/outline?id={InvoiceID}");

            DeleteButton.Visible = string.Equals(invoice.InvoiceStatus, InvoiceStatusEnum.Drafted.ToString(), StringComparison.OrdinalIgnoreCase) && !HasTransactionCode(invoice.Payments);
            DeleteButton.NavigateUrl = $"/admin/sales/invoices/delete?id={InvoiceID}";
        }

        private string GetInvoiceStatus(string status)
            => ProductHelper.GetInvoiceStatus(status);

        private void InvoiceItemGrid_Refreshed(object sender, EventArgs e)
        {
            SetupItemSection();
        }

        private void PrintInvoiceEventButton_Click(object sender, EventArgs e)
        {
            var (data, fileName) = InvoiceEventReport.PrintByInvoice(InvoiceID.Value, InvoiceEventReportType.Receipt);

            Response.SendFile(fileName, "pdf", data);
        }

        private void SetupItemSection()
        {
            ItemSection.Visible = InvoiceItemGrid.RowCount > 0;
        }

        private string GetLocalTime(DateTimeOffset? item)
        {
            return item.Format(User.TimeZone, nullValue: "None", isHtml: true);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/report")
                ? $"contact={CustomerID}"
                : null;
        }

        private bool HasTransactionCode(ICollection<QPayment> payments)
        {
            if (payments == null || payments.Count == 0)
                return false;

            foreach (var payment in payments)
                if (payment.TransactionId.IsNotEmpty())
                    return true;

            return false;
        }
    }
}