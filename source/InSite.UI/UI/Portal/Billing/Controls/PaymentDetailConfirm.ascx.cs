using System.Web.UI;

using InSite.Domain.Payments;
using InSite.UI.Portal.Billing.Utilities;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class PaymentDetailConfirm : UserControl
    {
        public PaymentInput GetInputValues(string invoiceNumber)
        {
            var payment = new PaymentInput(
                invoiceNumber,
                decimal.Parse(PaymentAmount.Text),
                new UnmaskedCreditCard(),
                new BillingAddress(),
                Page.Request.UserHostAddress
            );

            CardDetailConfirm.GetInputValues(payment.Card, payment.BillingAddress);

            return payment;
        }

        public void SetInputValues(PaymentInput payment)
        {
            PaymentAmount.Text = payment.Amount.ToString("n2");

            CardDetailConfirm.SetInputValues(payment.Card, payment.BillingAddress);
        }

        public void SetTaxBreakdown(decimal subtotal, decimal tax, decimal? rate)
        {
            var hasTax = tax > 0m;

            TaxPanel.Visible = hasTax;

            if (hasTax)
            {
                SubtotalAmount.Text = subtotal.ToString("n2");
                TaxRateDisplay.Text = rate.HasValue ? " (" + TaxHelper.FormatRate(rate.Value) + ")" : null;
                TaxAmount.Text = tax.ToString("n2");
            }
        }
    }
}
