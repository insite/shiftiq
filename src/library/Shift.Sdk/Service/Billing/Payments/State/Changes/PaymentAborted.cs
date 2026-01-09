using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class PaymentAborted : Change
    {
        public PaymentAborted(Guid payment, ErrorResponse response)
        {
            Payment = payment;
            Response = response;
        }

        public Guid Payment { get; set; }
        public ErrorResponse Response { get; }
    }
}