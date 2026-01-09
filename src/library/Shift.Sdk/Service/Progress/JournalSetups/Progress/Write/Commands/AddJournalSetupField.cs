using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.JournalSetups.Write
{
    public class AddJournalSetupField : Command
    {
        public Guid Field { get; }
        public JournalSetupFieldType Type { get; }
        public int Sequence { get; }
        public bool IsRequired { get; }

        public AddJournalSetupField(Guid journalSetup, Guid field, JournalSetupFieldType type, int sequence, bool isRequired)
        {
            AggregateIdentifier = journalSetup;
            Field = field;
            Type = type;
            Sequence = sequence;
            IsRequired = isRequired;
        }
    }
}
