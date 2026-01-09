using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupGroupRemoved : Change
    {
        public Guid Group { get; }

        public JournalSetupGroupRemoved(Guid group)
        {
            Group = group;
        }
    }
}
