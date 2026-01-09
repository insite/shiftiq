using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Read;
using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Payments.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QPaymentFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public Guid InvoiceIdentifier { get; set; }
            public Guid PaymentIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }

            public string TransactionIdentifier { get; set; }
            public string CustomerIP { get; set; }
            public string PaymentStatus { get; set; }
            public string ResultCode { get; set; }
            public string ResultMessage { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }

            public decimal PaymentAmount { get; set; }

            public int? InvoiceNumber { get; set; }

            public DateTimeOffset? PaymentAborted { get; set; }
            public DateTimeOffset? PaymentApproved { get; set; }
            public DateTimeOffset? PaymentDeclined { get; set; }
            public DateTimeOffset? PaymentStarted { get; set; }

            public Guid CreatedBy { get; set; }
            public string Employer { get; internal set; }
            public string ProductName { get; set; }
        }


        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        protected override int SelectCount(QPaymentFilter filter)
        {
            return ServiceLocator.PaymentSearch.CountPayments(filter);
        }

        protected override IListSource SelectData(QPaymentFilter filter)
        {
            filter.OrderBy = "PaymentStarted desc";

            return ServiceLocator.PaymentSearch
                .GetPayments(filter, x=>x.CreatedByUser, x => x.CreatedInvoice, x => x.CreatedInvoice.InvoiceItems.Select(y => y.Product))
                .ToSearchResult();
        }

        protected string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            if (when == null)
                return string.Empty;
            return TimeZones.Format(when.Value, User.TimeZone, true);
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e.Row))
                return;

            var payment = (QPayment)e.Row.DataItem;

            if (payment == null)
                return;

            var invoice = payment.CreatedInvoice;

            if(invoice == null) 
                return;

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

        #region Export

        public override IListSource GetExportData(QPaymentFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<QPayment>().Select(x => new ExportDataItem
            {
                CreatedBy= x.CreatedBy,
                CustomerIP= x.CustomerIP,
                InvoiceIdentifier= x.InvoiceIdentifier,
                OrganizationIdentifier= x.OrganizationIdentifier,
                PaymentAborted= x.PaymentAborted,
                PaymentApproved= x.PaymentApproved,
                PaymentAmount= x.PaymentAmount,
                PaymentStatus= x.PaymentStatus,
                PaymentDeclined= x.PaymentDeclined,
                PaymentIdentifier= x.PaymentIdentifier,
                PaymentStarted= x.PaymentStarted,
                ResultCode= x.ResultCode,
                ResultMessage= x.ResultMessage,
                TransactionIdentifier= x.TransactionId,
                CustomerEmail = x.CreatedByUser.UserEmail,
                CustomerName = x.CreatedByUser.UserFullName,
                InvoiceNumber = x.CreatedInvoice.InvoiceNumber,
                Employer = x.CreatedInvoice.CustomerEmployer,
                ProductName = GetExportProducts(x.CreatedInvoice.InvoiceItems)
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