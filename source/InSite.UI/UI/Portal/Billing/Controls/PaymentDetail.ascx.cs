using System;
using System.Web.UI;

using InSite.Domain.Payments;
using InSite.UI.Portal.Billing.Utilities;

using Shift.Common.Events;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class PaymentDetail : UserControl
    {
        public event StringValueHandler ProvinceChanged;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CardDetail.ProvinceChanged += (s, ev) => ProvinceChanged?.Invoke(this, ev);
        }

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

        public void SetInputValues(decimal subtotal, decimal tax, decimal? taxRate, bool hidePaymentAmount)
        {
            var total = subtotal + tax;
            var hasTax = tax > 0m;

            PaymentAmountPanel.Visible = !hidePaymentAmount;
            PaymentAmountDisplay.Text = total.ToString("c2");
            PaymentAmountValue.Value = total.ToString();

            TaxPanel.Visible = hasTax;

            if (hasTax)
            {
                SubtotalDisplay.Text = subtotal.ToString("c2");
                TaxRateDisplay.Text = taxRate.HasValue ? " (" + TaxHelper.FormatRate(taxRate.Value) + ")" : null;
                TaxAmountDisplay.Text = tax.ToString("c2");
            }
        }

        public void Clear()
        {
            PaymentAmountDisplay.Text = null;
            PaymentAmountValue.Value = null;

            TaxPanel.Visible = false;
            SubtotalDisplay.Text = null;
            TaxRateDisplay.Text = null;
            TaxAmountDisplay.Text = null;

            CardDetail.Clear();
        }
    }
}
