using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceEmployerChanged : Change
    {
        public Guid Experience { get; }
        public string Employer { get; }

        public ExperienceEmployerChanged(Guid experience, string employer)
        {
            Experience = experience;
            Employer = employer;
        }
    }
}
