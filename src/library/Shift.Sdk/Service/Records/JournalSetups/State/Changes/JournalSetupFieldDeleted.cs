using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupFieldDeleted : Change
    {
        public Guid Field { get; }

        public JournalSetupFieldDeleted(Guid field)
        {
            Field = field;
        }
    }
}
