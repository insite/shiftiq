using System;
using System.Linq;

using Humanizer;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Payments.Forms
{
    public partial class Outline : InSite.UI.Layout.Admin.AdminBasePage
    {
        private Guid? PaymentID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

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
            if (PaymentID == null)
            {
                HttpResponseHelper.Redirect("/ui/admin/sales/payments/search");
                return;
            }

            var payment = ServiceLocator.PaymentSearch.GetPayment(PaymentID.Value, x => x.CreatedInvoice, x => x.CreatedInvoice.InvoiceItems.Select(y => y.Product));
            if (payment == null || payment.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/sales/payments/search");
                return;
            }

            var title = $"Payment of <a href='/ui/admin/sales/invoices/outline?id={payment.InvoiceIdentifier}'>Invoice</a>";

            string subtitle;

            if (payment.PaymentApproved.HasValue)
                subtitle = $"Approved {payment.PaymentApproved.Value.Humanize()}";
            else if (payment.PaymentAborted.HasValue)
                subtitle = $"Aborted {payment.PaymentAborted.Value.Humanize()}";
            else if (payment.PaymentDeclined.HasValue)
                subtitle = $"Declined {payment.PaymentDeclined.Value.Humanize()}";
            else
                subtitle = $"Started {payment.PaymentStarted.Value.Humanize()}";

            var userName = UserSearch.GetFullName(payment.CreatedBy);
            subtitle += $" by {userName}";

            PageHelper.AutoBindHeader(this, qualifier: $"{title} <span class='form-text'>{subtitle}</span>");

            PaymentAmount.Text = payment.PaymentAmount.ToString("c2");
            PaymentStatus.Text = payment.PaymentStatus;
            CustomerIP.Text = payment.CustomerIP;
            ResultCode.Text = payment.ResultCode;
            ResultMessage.Text = payment.ResultMessage;


            PaymentStarted.Text = GetLocalTime(payment.PaymentStarted);
            PaymentAborted.Text = GetLocalTime(payment.PaymentAborted);
            PaymentDeclined.Text = GetLocalTime(payment.PaymentDeclined);
            PaymentApproved.Text = GetLocalTime(payment.PaymentApproved);

            BindProducts(payment);
        }

        private void BindProducts(Application.Payments.Read.QPayment payment)
        {
            var invoice = payment.CreatedInvoice;

            if (invoice == null)
                return;

            var productNames = invoice.InvoiceItems
                .Where(x => x.Product != null)
                .Select(x => x.Product.ProductName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ProductName.Text = string.Join(", ", productNames);
        }

        #region Helper methods

        private string GetLocalTime(DateTimeOffset? item)
        {
            return item.Format(User.TimeZone, nullValue: "None", isHtml: true);
        }

        #endregion
    }
}