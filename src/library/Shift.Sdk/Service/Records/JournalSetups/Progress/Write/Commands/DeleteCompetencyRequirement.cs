using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class DeleteCompetencyRequirement : Command
    {
        public Guid Competency { get; }

        public DeleteCompetencyRequirement(Guid journalSetup, Guid competency)
        {
            AggregateIdentifier = journalSetup;
            Competency = competency;
        }
    }
}
