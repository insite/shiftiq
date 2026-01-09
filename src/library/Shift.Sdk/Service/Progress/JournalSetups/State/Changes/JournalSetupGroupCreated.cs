using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupGroupCreated : Change
    {
        public Guid Group { get; }

        public JournalSetupGroupCreated(Guid group)
        {
            Group = group;
        }
    }
}
