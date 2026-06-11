using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class DeleteJournalSetup : Command
    {
        public DeleteJournalSetup(Guid journalSetup)
        {
            AggregateIdentifier = journalSetup;
        }
    }
}
