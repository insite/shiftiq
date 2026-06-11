using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CriterionDeleted : Change
    {
        public Guid Criterion { get; set; }

        public CriterionDeleted(Guid sieve)
        {
            Criterion = sieve;
        }
    }
}
