using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Payments
{
    public class Gateway : AggregateState
    {
        public GatewayStatus Status { get; set; }
        public GatewayType Type { get; set; }

        public string Name { get; set; }

        public Dictionary<Guid, Payment> Payments { get; set; }

        public Payment FindPayment(Guid payment)
        {
            if (PaymentExists(payment))
                return Payments[payment];
            return null;
        }

        public bool PaymentExists(Guid payment)
        {
            return Payments.Any(x => x.Key == payment);
        }

        public void When(GatewayOpened e)
        {
            Status = GatewayStatus.Open;
            Type = e.Type;
            Name = e.Name;
            Payments = new Dictionary<Guid, Payment>();
        }

        public void When(GatewayClosed _)
        {
            Status = GatewayStatus.Closed;
        }

        public void When(MerchantAdded e)
        {

        }

        public void When(MerchantRemoved e)
        {

        }

        public void When(MerchantActivated e)
        {

        }

        public void When(MerchantDeactivated e)
        {

        }

        public void When(PaymentImported e)
        {
            var payment = new Payment(e.OrderNumber, e.Amount, null, null, e.CustomerIP)
            {
                Status = e.Status,
                CreatedBy = e.OriginUser
            };
            Payments.Add(e.Payment, payment);
        }

        public void When(PaymentStarted e)
        {
            var payment = new Payment(e.Request.OrderNumber, e.Request.Amount, e.Request.Card, e.Request.BillingAddress, e.Request.CustomerIP)
            {
                Status = PaymentStatus.Started,
                CreatedBy = e.OriginUser
            };
            Payments.Add(e.Payment, payment);
        }

        public void When(PaymentAborted e)
        {
            var payment = Payments[e.Payment];
            payment.Status = PaymentStatus.Aborted;
            payment.Error = e.Response;
        }

        public void When(PaymentCompleted e)
        {
            var payment = Payments[e.Payment];
            payment.Status = PaymentStatus.Completed;
            payment.Response = e.Response;
        }

        public void When(PaymentCreatedByModified _) { }
    }
}
