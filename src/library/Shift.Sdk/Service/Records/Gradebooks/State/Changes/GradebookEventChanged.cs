using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookEventChanged : Change
    {
        public GradebookEventChanged(Guid? @event)
        {
            Event = @event;
        }

        public Guid? Event { get; set; }
    }
}
