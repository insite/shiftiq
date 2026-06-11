using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookEventAdded : Change
    {
        public Guid Event { get; set; }
        public bool IsPrimary { get; set; }

        public GradebookEventAdded(Guid @event, bool isPrimary)
        {
            Event = @event;
            IsPrimary = isPrimary;
        }
    }
}
