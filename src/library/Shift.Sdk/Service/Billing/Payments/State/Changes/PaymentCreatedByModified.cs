using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class PaymentCreatedByModified : Change
    {
        public Guid Payment { get; set; }
        public Guid CreatedBy { get; set; }

        public PaymentCreatedByModified(Guid payment, Guid createdBy)
        {
            Payment = payment;
            CreatedBy = createdBy;
        }
    }
}
