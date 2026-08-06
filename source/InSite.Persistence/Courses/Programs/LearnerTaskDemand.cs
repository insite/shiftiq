using System;

namespace InSite.Persistence
{
    /// <summary>
    /// One program's claim on one achievement for one learner. Several programs can make competing
    /// claims, which the cascade resolves with a most-restrictive merge.
    /// </summary>
    public class LearnerTaskDemand
    {
        public Guid LearnerUserIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }

        public bool IsRequired { get; set; }
        public bool IsPlanned { get; set; }

        public int? LifetimeMonths { get; set; }
    }
}
