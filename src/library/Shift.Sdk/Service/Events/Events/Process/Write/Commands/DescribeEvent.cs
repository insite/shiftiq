using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Events;

using Shift.Common;

namespace InSite.Application.Events.Write
{
    public class DescribeEvent : Command
    {
        public MultilingualString Title { get; set; }
        public MultilingualString Summary { get; set; }
        public MultilingualString Description { get; set; }
        public MultilingualString MaterialsForParticipation { get; set; }
        public EventInstruction[] Instructions { get; set; }
        public MultilingualString ClassLink { get; set; }

        public DescribeEvent(Guid id, MultilingualString title, MultilingualString summary, MultilingualString description, MultilingualString materialsForParticipation, EventInstruction[] instructions, MultilingualString classLink)
        {
            AggregateIdentifier = id;
            Title = title;
            Summary = summary;
            Description = description;
            MaterialsForParticipation = materialsForParticipation;
            Instructions = instructions;
            ClassLink = classLink;
        }
    }
}
