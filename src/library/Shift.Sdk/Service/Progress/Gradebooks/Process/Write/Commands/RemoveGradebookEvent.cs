using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class RemoveGradebookEvent : Command
    {
        public RemoveGradebookEvent(Guid gradebook, Guid @event)
        {
            AggregateIdentifier = gradebook;
            Event = @event;
        }

        public Guid Event { get; set; }
    }
}
