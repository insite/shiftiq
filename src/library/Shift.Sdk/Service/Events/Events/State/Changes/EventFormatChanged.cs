using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventFormatChanged : Change
    {
        public string Format { get; set; }

        public EventFormatChanged(string format)
        {
            Format = format;
        }
    }
}