using System;
using System.Collections.Generic;

using InSite.Application.Invoices.Read;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sales.Invoices
{
    public partial class Search : SearchPage<VInvoiceFilter>
    {
        public override string EntityName => "Invoice";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Draft Invoice", "/ui/admin/sales/invoices/draft", null, null));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("CustomerEmail", "Customer Email"),
                new DownloadColumn("CustomerFullName", "Customer Full Name"),
                new DownloadColumn("CustomerIdentifier", "Customer Identifier"),
                new DownloadColumn("InvoiceAmount", "Invoice Amount"),
                new DownloadColumn("InvoiceDrafted", "Invoice Drafted"),
                new DownloadColumn("InvoiceIdentifier", "Invoice Identifier"),
                new DownloadColumn("InvoiceNumber", "Invoice Number"),
                new DownloadColumn("InvoicePaid", "Invoice Paid"),
                new DownloadColumn("InvoiceStatus", "Invoice Status"),
                new DownloadColumn("InvoiceSubmitted", "Invoice Submitted"),
                new DownloadColumn("ItemCount", "Item Count"),
                new DownloadColumn("OrganizationIdentifier", "Organization Identifier"),
                new DownloadColumn("TransactionCode", "Transaction Code"),
                new DownloadColumn("CustomerEmployer", "Customer Employer"),
                new DownloadColumn("CustomerPersonCode", LabelHelper.GetLabelContentText("Person Code")),
                new DownloadColumn("Products", "Products"),
            };
        }
    }
}