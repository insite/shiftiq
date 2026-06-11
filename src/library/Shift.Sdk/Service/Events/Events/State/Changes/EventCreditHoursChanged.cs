using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCreditHoursChanged : Change
    {
        public decimal? CreditHours { get; set; }

        public EventCreditHoursChanged(decimal? creditHours)
        {
            CreditHours = creditHours;
        }
    }
}
