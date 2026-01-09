using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class CreateJournal : Command
    {
        public Guid JournalSetup { get; }
        public Guid User { get; }

        public CreateJournal(Guid journal, Guid journalSetup, Guid user)
        {
            AggregateIdentifier = journal;
            JournalSetup = journalSetup;
            User = user;
        }
    }
}
