using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupFramework : Command
    {
        public Guid? Framework { get; }

        public ChangeJournalSetupFramework(Guid journalSetup, Guid? framework)
        {
            AggregateIdentifier = journalSetup;
            Framework = framework;
        }
    }
}
