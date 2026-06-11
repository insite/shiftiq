using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Events
{
    public class AppointmentDescribed : Change
    {
        public MultilingualString Title { get; set; }
        public MultilingualString Description { get; set; }

        public AppointmentDescribed(MultilingualString title, MultilingualString description)
        {
            Title = title;
            Description = description;
        }
    }
}
