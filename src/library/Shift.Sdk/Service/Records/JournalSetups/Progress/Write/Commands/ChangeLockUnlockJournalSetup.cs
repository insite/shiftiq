using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeLockUnlockJournalSetup : Command
    {
        public DateTimeOffset? JournalSetupLocked { get; }

        public ChangeLockUnlockJournalSetup(Guid journalSetup, DateTimeOffset? journalSetupLocked)
        {
            AggregateIdentifier = journalSetup;
            JournalSetupLocked = journalSetupLocked;
        }
    }
}
