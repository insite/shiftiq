using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class CreateJournalSetupGroup : Command
    {
        public Guid Group { get; }

        public CreateJournalSetupGroup(Guid journalSetup, Guid group)
        {
            AggregateIdentifier = journalSetup;
            Group = group;
        }
    }
}
