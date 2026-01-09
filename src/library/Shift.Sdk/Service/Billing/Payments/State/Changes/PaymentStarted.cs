using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class PaymentStarted : Change
    {
        public PaymentStarted(Guid tenant, Guid invoice, Guid payment, PaymentRequest request)
        {
            Tenant = tenant;
            Invoice = invoice;
            Payment = payment;
            Request = request;
        }

        public Guid Tenant { get; set; }
        public Guid Invoice { get; set; }
        public Guid Payment { get; set; }
        public PaymentRequest Request { get; set; }
    }
}