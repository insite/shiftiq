using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class RenameJournalSetup : Command
    {
        public string Name { get; }

        public RenameJournalSetup(Guid journalSetup, string name)
        {
            AggregateIdentifier = journalSetup;
            Name = name;
        }
    }
}
