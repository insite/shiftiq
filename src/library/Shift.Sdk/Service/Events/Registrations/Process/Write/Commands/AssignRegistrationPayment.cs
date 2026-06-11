using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignRegistrationPayment : Command
    {
        public Guid Payment { get; set; }

        public AssignRegistrationPayment(Guid registration, Guid payment)
        {
            AggregateIdentifier = registration;
            Payment = payment;
        }
    }
}
