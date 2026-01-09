using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionIsRangeModified : Change
    {
        public Guid RubricCriterionId { get; set; }
        public bool IsRange { get; set; }

        public RubricCriterionIsRangeModified(Guid rubricCriterionId, bool isRange)
        {
            RubricCriterionId = rubricCriterionId;
            IsRange = isRange;
        }
    }
}
