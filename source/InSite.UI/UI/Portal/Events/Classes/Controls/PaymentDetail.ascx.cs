using System;
using System.Web.UI;

using InSite.Domain.Payments;
using InSite.UI.Portal.Events.Classes.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class PaymentDetail : UserControl
    {
        public class Payment
        {
            public decimal PaymentAmount { get; set; }
            public UnmaskedCreditCard Card { get; set; }
            public BillingAddress BillingAddress { get; set; }
            public string BillingCode { get; set; }
        }

        private decimal PaymentAmount
        {
            get => (decimal?)ViewState[nameof(PaymentAmount)] ?? 0;
            set => ViewState[nameof(PaymentAmount)] = value;
        }

        public PaymentMode Mode
        {
            get => (PaymentMode)(ViewState[nameof(Mode)] ?? PaymentMode.Card);
            set
            {
                ViewState[nameof(Mode)] = value;
                OnModeUpdated();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                OnModeUpdated();
        }

        public Payment GetInputValues()
        {
            var payment = new Payment { PaymentAmount = PaymentAmount };

            switch (Mode)
            {
                case PaymentMode.Card: payment.Card = CardDetail.GetInputValues(); break;
                case PaymentMode.BillTo: payment.BillingCode = BillingCode.Text;; break;
                default: throw ApplicationError.Create("Unexpected payment mode: {0}", Mode.GetName());
            }

            return payment;
        }

        public void SetInputValues(decimal paymentAmount, bool hidePaymentAmount)
        {
            PaymentAmount = paymentAmount;
            PaymentAmountPanel.Visible = !hidePaymentAmount;
            PaymentAmountLiteral.Text = paymentAmount.ToString("c2");

            CardDetail.Clear();

            BillingCode.Text = null;
        }

        private void OnModeUpdated()
        {
            switch (Mode)
            {
                case PaymentMode.Card: MultiView.SetActiveView(CardView); break;
                case PaymentMode.BillTo: MultiView.SetActiveView(BillToView); break;
                default: throw ApplicationError.Create("Unexpected payment mode: {0}", Mode.GetName());
            }
        }
    }
}