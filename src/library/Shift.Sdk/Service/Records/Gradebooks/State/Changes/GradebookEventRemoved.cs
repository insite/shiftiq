using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookEventRemoved : Change
    {
        public Guid Event { get; set; }
        public Guid? NewPrimaryEvent { get; set; }

        public GradebookEventRemoved(Guid @event, Guid? newPrimaryEvent)
        {
            Event = @event;
            NewPrimaryEvent = newPrimaryEvent;
        }
    }
}
