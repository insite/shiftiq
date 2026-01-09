using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventRenumbered : Change
    {
        public int Number { get; set; }

        public EventRenumbered(int number)
        {
            Number = number;
        }
    }

    public class EventRetitled : Change
    {
        public string Title { get; set; }

        public EventRetitled(string title)
        {
            Title = title;
        }
    }
}