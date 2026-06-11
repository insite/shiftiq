using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ModifyRegistrationBillingCode : Command
    {
        public string BillingCode { get; set; }

        public ModifyRegistrationBillingCode(Guid registration, string billingCode)
        {
            AggregateIdentifier = registration;
            BillingCode = billingCode;
        }
    }
}
