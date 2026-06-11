using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventMessagePeriodModified : Change
    {
        public int? SendReminderBeforeDays { get; set; }

        public EventMessagePeriodModified(int? sendReminderBeforeDays)
        {
            SendReminderBeforeDays = sendReminderBeforeDays;
        }
    }
}
