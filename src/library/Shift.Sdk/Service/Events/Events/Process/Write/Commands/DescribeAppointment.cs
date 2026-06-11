using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Events.Write
{
    public class DescribeAppointment : Command
    {
        public MultilingualString Title { get; set; }
        public MultilingualString Description { get; set; }

        public DescribeAppointment(Guid id, MultilingualString title, MultilingualString description)
        {
            AggregateIdentifier = id;
            Title = title;
            Description = description;
        }
    }
}
