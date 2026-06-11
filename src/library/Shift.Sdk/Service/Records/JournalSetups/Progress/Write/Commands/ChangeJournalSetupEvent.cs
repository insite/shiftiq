using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupEvent : Command
    {
        public Guid? Event { get; }

        public ChangeJournalSetupEvent(Guid journalSetup, Guid? @event)
        {
            AggregateIdentifier = journalSetup;
            Event = @event;
        }
    }
}
