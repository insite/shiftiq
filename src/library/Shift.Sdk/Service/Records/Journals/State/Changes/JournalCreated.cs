using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalCreated : Change
    {
        public Guid JournalSetup { get; }
        public Guid User { get; }

        public JournalCreated(Guid journalSetup, Guid user)
        {
            JournalSetup = journalSetup;
            User = user;
        }
    }
}
