using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRemoved : Change
    {
        public Guid RubricCriterionId { get; set; }

        public RubricCriterionRemoved(Guid rubricCriterionId)
        {
            RubricCriterionId = rubricCriterionId;
        }
    }
}
