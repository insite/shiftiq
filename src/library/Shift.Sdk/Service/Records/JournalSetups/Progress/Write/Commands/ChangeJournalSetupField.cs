using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupField : Command
    {
        public Guid Field { get; }
        public bool IsRequired { get;}

        public ChangeJournalSetupField(Guid journalSetup, Guid field, bool isRequired)
        {
            AggregateIdentifier = journalSetup;
            Field = field;
            IsRequired = isRequired;
        }
    }
}
