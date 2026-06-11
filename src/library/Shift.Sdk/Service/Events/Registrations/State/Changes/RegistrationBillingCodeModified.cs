using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationBillingCodeModified : Change
    {
        public string BillingCode { get; set; }

        public RegistrationBillingCodeModified(string billingCode)
        {
            BillingCode = billingCode;
        }
    }
}
