using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceEmployer : Command
    {
        public Guid Experience { get; }
        public string Employer { get; }

        public ChangeExperienceEmployer(Guid journal, Guid experience, string employer)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Employer = employer;
        }
    }
}
