using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRatingRemoved : Change
    {
        public Guid RubricRatingId { get; set; }

        public RubricCriterionRatingRemoved(Guid rubricRatingId)
        {
            RubricRatingId = rubricRatingId;
        }
    }
}
