using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ChangeJournalSetupAchievement : Command
    {
        public Guid? Achievement { get; }

        public ChangeJournalSetupAchievement(Guid journalSetup, Guid? achievement)
        {
            AggregateIdentifier = journalSetup;
            Achievement = achievement;
        }
    }
}
