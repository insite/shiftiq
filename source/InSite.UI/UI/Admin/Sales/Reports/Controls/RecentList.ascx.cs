using System;
using System.Linq;

using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Invoices.Reports.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(QPaymentFilter filter, int count)
        {
            var payments = ServiceLocator.PaymentSearch
                .GetRecentPayments(filter, count, x => x.CreatedInvoice)
                .Select(x => new
                {
                    x.PaymentIdentifier,
                    x.CreatedInvoice.InvoiceIdentifier,
                    InvoiceNumber = x.CreatedInvoice.InvoiceNumber.HasValue ? $"{x.CreatedInvoice.InvoiceNumber}" : x.CreatedInvoice.InvoiceIdentifier.ToString(),
                    InvoiceCustomerName = x.CreatedInvoice.CustomerFullName,
                    x.CreatedInvoice.InvoiceAmount,
                    PaymentStatus = x.PaymentStatus == "Completed" ? "paid" : "try to pay",
                    PaymentAmount = x.PaymentAmount.ToString("c2"),
                    x.PaymentStarted
                })
                .ToList();

            ItemCount = payments.Count;

            PaymentRepeater.DataSource = payments;
            PaymentRepeater.DataBind();
        }

        protected static string GetTimestampHtml(object o)
        {
            if (o == null)
                return "some time ago";

            DateTimeOffset when = (DateTimeOffset)o;

            if (when == DateTimeOffset.MinValue)
                return "some time ago";

            return $"{Shift.Common.Humanizer.Humanize(when)}";
        }
    }
}