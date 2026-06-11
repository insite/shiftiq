using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupEventChanged : Change
    {
        public Guid? Event { get; }

        public JournalSetupEventChanged(Guid? @event)
        {
            Event = @event;
        }
    }
}
