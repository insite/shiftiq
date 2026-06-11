using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceAdded : Change
    {
        public Guid Experience { get; }

        public ExperienceAdded(Guid experience)
        {
            Experience = experience;
        }
    }
}
