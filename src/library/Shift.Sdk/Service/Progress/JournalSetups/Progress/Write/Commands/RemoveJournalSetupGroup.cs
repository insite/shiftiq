using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class RemoveJournalSetupGroup : Command
    {
        public Guid Group { get; }

        public RemoveJournalSetupGroup(Guid journalSetup, Guid group)
        {
            AggregateIdentifier = journalSetup;
            Group = group;
        }
    }
}
