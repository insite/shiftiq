using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRatingPointsModified : Change
    {
        public Guid RubricRatingId { get; set; }
        public decimal RatingPoints { get; set; }

        public RubricCriterionRatingPointsModified(Guid rubricRatingId, decimal ratingPoints)
        {
            RubricRatingId = rubricRatingId;
            RatingPoints = ratingPoints;
        }
    }
}
