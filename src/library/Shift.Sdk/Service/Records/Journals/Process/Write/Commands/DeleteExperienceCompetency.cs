using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class DeleteExperienceCompetency : Command
    {
        public Guid Experience { get; }
        public Guid Competency { get; }

        public DeleteExperienceCompetency(Guid journal, Guid experience, Guid competency)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Competency = competency;
        }
    }
}
