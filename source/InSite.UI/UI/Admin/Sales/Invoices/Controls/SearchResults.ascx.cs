using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Read;
using InSite.Application.Payments.Read;
using InSite.Common;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Invoices.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VInvoiceFilter>
    {
        #region Classes
        public class ExportDataItem
        {
            public Guid CustomerIdentifier { get; set; }
            public Guid InvoiceIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }

            public int? InvoiceNumber { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerFullName { get; set; }
            public string InvoiceStatus { get; set; }
            public string TransactionCode { get; set; }

            public int? ItemCount { get; set; }

            public decimal? InvoiceAmount { get; set; }

            public DateTimeOffset? InvoiceDrafted { get; set; }
            public DateTimeOffset? InvoicePaid { get; set; }
            public DateTimeOffset? InvoiceSubmitted { get; set; }

            public string CustomerEmployer { get; set; }
            public string CustomerPersonCode { get; set; }

            public string Products { get; set; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SetContentLabelHeaders(e);

            if (!IsContentItem(e.Row))
                return;

            var invoice = (VInvoice)e.Row.DataItem;

            var deleteButton = e.Row.FindControl("DeleteButton");
            deleteButton.Visible = string.Equals(invoice.InvoiceStatus, InvoiceStatus.Drafted.ToString(), StringComparison.OrdinalIgnoreCase) && !HasTransactionCode(invoice.Payments);

            var products = invoice.InvoiceItems
                .Where(x => x.Product != null)
                .Select(x => new
                {
                    ProductName = x.Product.ProductName
                })
                .Distinct()
                .OrderBy(x => x.ProductName)
                .ToList();

            var productRepeater = (Repeater)e.Row.FindControl("ProductRepeater");
            productRepeater.DataSource = products;
            productRepeater.DataBind();
        }

        protected override int SelectCount(VInvoiceFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountInvoices(filter);
        }

        protected override IListSource SelectData(VInvoiceFilter filter)
        {
            return ServiceLocator.InvoiceSearch
                .GetInvoices(filter, x => x.Payments, x => x.InvoiceItems.Select(y => y.Product))
                .ToSearchResult();
        }

        protected string GetInvoiceStatus(object value)
        {
            var status = (string)value;

            return status.IsEmpty()
                ? string.Empty
                : status.ToEnum<InvoiceStatusType>().GetDescription();
        }

        protected string GetLocalTime(object item)
        {
            return ((DateTimeOffset?)item).Format(User.TimeZone, true);
        }

        protected string GetTransactionCode(object item)
        {
            var payments = (ICollection<QPayment>)item;

            var transactionId = new StringBuilder();

            if (payments != null && payments.Count > 0)
            {
                foreach (var payment in payments)
                {
                    if (payment.TransactionId.IsNotEmpty())
                        transactionId.Append($"{payment.TransactionId}<br>");
                }
            }

            return transactionId.ToString();
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

        private void SetContentLabelHeaders(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
        }

        #region Export

        public override IListSource GetExportData(VInvoiceFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<VInvoice>().Select(x => new ExportDataItem
            {
                CustomerEmail = x.CustomerEmail,
                CustomerFullName = x.CustomerFullName,
                CustomerIdentifier = x.CustomerIdentifier,
                InvoiceAmount = x.InvoiceAmount,
                InvoiceDrafted = x.InvoiceDrafted,
                InvoiceIdentifier = x.InvoiceIdentifier,
                InvoiceNumber = x.InvoiceNumber,
                InvoicePaid = x.InvoicePaid,
                InvoiceStatus = x.InvoiceStatus,
                InvoiceSubmitted = x.InvoiceSubmitted,
                ItemCount = x.ItemCount,
                OrganizationIdentifier = x.OrganizationIdentifier,
                TransactionCode = String.Join(";", x.Payments.Select(s => s.TransactionId).ToArray()),
                CustomerEmployer = x.CustomerEmployer,
                CustomerPersonCode = x.CustomerPersonCode,
                Products = GetExportProducts(x.InvoiceItems)

            }).ToList().ToSearchResult();
        }

        private static string GetExportProducts(ICollection<QInvoiceItem> invoiceItems)
        {
            var products = invoiceItems
                .Where(x => x.Product != null)
                .Select(x => x.Product.ProductName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return string.Join(", ", products);
        }

        #endregion
    }
}