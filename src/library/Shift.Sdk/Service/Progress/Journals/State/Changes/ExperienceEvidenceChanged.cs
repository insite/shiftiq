using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceEvidenceChanged : Change
    {
        public Guid Experience { get; }
        public string Evidence { get; }

        public ExperienceEvidenceChanged(Guid experience, string evidence)
        {
            Experience = experience;
            Evidence = evidence;
        }
    }
}
