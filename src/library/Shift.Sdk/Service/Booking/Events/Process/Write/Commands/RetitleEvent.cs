using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RetitleEvent : Command
    {
        public string EventTitle { get; set; }

        public RetitleEvent(Guid id, string title)
        {
            AggregateIdentifier = id;
            EventTitle = title;
        }
    }
}
