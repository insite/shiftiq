using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceEvidence : Command
    {
        public Guid Experience { get; }
        public string Evidence { get; }

        public ChangeExperienceEvidence(Guid journal, Guid experience, string evidence)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Evidence = evidence;
        }
    }
}
