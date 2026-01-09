using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Events
{
    public class EventDescribed : Change
    {
        public MultilingualString Title { get; set; }
        public MultilingualString Summary { get; set; }
        public MultilingualString Description { get; set; }
        public MultilingualString MaterialsForParticipation { get; set; }
        public EventInstruction[] Instructions { get; set; }
        public MultilingualString ClassLink { get; set; }

        public EventDescribed(MultilingualString title, MultilingualString summary, MultilingualString description, MultilingualString materialsForParticipation, EventInstruction[] instructions, MultilingualString classLink)
        {
            Title = title;
            Summary = summary;
            Description = description;
            MaterialsForParticipation = materialsForParticipation;
            Instructions = instructions;
            ClassLink = classLink;
        }
    }
}