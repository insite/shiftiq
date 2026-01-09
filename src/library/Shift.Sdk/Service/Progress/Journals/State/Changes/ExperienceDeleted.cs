using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceDeleted : Change
    {
        public Guid Experience { get; set; }

        public ExperienceDeleted(Guid experience)
        {
            Experience = experience;
        }
    }
}
