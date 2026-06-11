using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class PaymentCompleted : Change
    {
        public PaymentCompleted(Guid payment, PaymentResponse response)
        {
            Payment = payment;
            Response = response;
        }

        public Guid Payment { get; set; }
        public PaymentResponse Response { get; }
    }
}