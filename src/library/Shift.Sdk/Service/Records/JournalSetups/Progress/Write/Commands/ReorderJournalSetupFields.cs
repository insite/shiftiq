using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ReorderJournalSetupFields : Command
    {
        public (Guid FieldIdentifier, int Sequence)[] Fields { get; set; }

        public ReorderJournalSetupFields(Guid journalSetupIdentifier, (Guid FieldIdentifier, int Sequence)[] fields)
        {
            AggregateIdentifier = journalSetupIdentifier;
            Fields = fields;
        }
    }
}
