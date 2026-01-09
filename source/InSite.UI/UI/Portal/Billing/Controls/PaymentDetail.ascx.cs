using System.Web.UI;

using InSite.Domain.Payments;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class PaymentDetail : UserControl
    {
        public PaymentInput GetInputValues(string invoiceNumber)
        {
            var payment = new PaymentInput(
                invoiceNumber,
                decimal.Parse(PaymentAmountValue.Value), 
                new UnmaskedCreditCard(), 
                new BillingAddress(), 
                Page.Request.UserHostAddress
            );

            CardDetail.GetInputValues(payment.Card, payment.BillingAddress);

            return payment;
        }

        public void SetInputValues(decimal paymentAmount, bool hidePaymentAmount)
        {
            PaymentAmountPanel.Visible = !hidePaymentAmount;
            PaymentAmountDisplay.Text = paymentAmount.ToString("c2");
            PaymentAmountValue.Value = paymentAmount.ToString();
        }

        public void Clear()
        {
            PaymentAmountDisplay.Text = null;
            PaymentAmountValue.Value = null;

            CardDetail.Clear();
        }
    }
}