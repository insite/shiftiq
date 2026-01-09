using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class CriterionFilterDeleted : Change
    {
        public Guid Criterion { get; set; }

        public CriterionFilterDeleted(Guid criterion)
        {
            Criterion = criterion;
        }
    }
}
