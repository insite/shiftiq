using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class AddGradebookEvent : Command
    {
        public AddGradebookEvent(Guid gradebook, Guid @event, bool replacePrimary)
        {
            AggregateIdentifier = gradebook;
            Event = @event;
            ReplacePrimary = replacePrimary;
        }

        public Guid Event { get; set; }
        public bool ReplacePrimary { get; set; }
    }
}
