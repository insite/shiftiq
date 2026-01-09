using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class LockUnlockJournalSetupChanged : Change
    {
        public DateTimeOffset? JournalSetupLocked { get; }

        public LockUnlockJournalSetupChanged(DateTimeOffset? journalSetupLocked)
        {
            JournalSetupLocked = journalSetupLocked;
        }
    }
}