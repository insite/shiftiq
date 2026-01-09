using System.Web.UI;

using InSite.Domain.Payments;

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
    }
}