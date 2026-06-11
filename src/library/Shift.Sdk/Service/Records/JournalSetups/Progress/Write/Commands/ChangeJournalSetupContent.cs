using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupContent : Command
    {
        public ContentContainer Content { get; }

        public ChangeJournalSetupContent(Guid journalSetup, ContentContainer content)
        {
            AggregateIdentifier = journalSetup;
            Content = content;
        }
    }
}
