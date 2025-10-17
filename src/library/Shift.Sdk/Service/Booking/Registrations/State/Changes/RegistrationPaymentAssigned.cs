using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationPaymentAssigned : Change
    {
        public Guid Payment { get; set; }

        public RegistrationPaymentAssigned(Guid payment)
        {
            Payment = payment;
        }
    }
}
