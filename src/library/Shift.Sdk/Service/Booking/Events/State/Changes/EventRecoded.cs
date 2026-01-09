using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventRecoded : Change
    {
        public string ClassCode { get; set; }
        public string BillingCode { get; set; }

        public EventRecoded(string classCode, string billingCode)
        {
            ClassCode = classCode;
            BillingCode = billingCode;
        }
    }
}