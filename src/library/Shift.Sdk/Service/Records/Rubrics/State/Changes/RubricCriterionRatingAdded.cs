using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRatingAdded : Change
    {
        public Guid RubricCriterionId { get; set; }
        public Guid RubricRatingId { get; set; }
        public string RatingTitle { get; set; }
        public decimal RatingPoints { get; set; }
        public int? RatingSequence { get; set; }

        public RubricCriterionRatingAdded(Guid rubricCriterionId, Guid rubricRatingId, string ratingTitle, decimal ratingPoints, int? ratingSequence)
        {
            RubricCriterionId = rubricCriterionId;
            RubricRatingId = rubricRatingId;
            RatingTitle = ratingTitle;
            RatingPoints = ratingPoints;
            RatingSequence = ratingSequence;
        }
    }
}
