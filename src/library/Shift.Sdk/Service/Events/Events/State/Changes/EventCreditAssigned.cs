
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCreditAssigned : Change
    {
        public decimal Hours { get; set; }

        public EventCreditAssigned(decimal hours)
        {
            Hours = hours;
        }
    }
}