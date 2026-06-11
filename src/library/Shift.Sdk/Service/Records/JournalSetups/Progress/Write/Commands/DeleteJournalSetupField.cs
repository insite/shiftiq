using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class DeleteJournalSetupField : Command
    {
        public Guid Field { get; }

        public DeleteJournalSetupField(Guid journalSetup, Guid field)
        {
            AggregateIdentifier = journalSetup;
            Field = field;
        }
    }
}
