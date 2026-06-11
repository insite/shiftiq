using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupFieldContent : Command
    {
        public Guid Field { get; }
        public ContentContainer Content { get; }

        public ChangeJournalSetupFieldContent(Guid journalSetup, Guid field, ContentContainer content)
        {
            AggregateIdentifier = journalSetup;
            Field = field;
            Content = content;
        }
    }
}
