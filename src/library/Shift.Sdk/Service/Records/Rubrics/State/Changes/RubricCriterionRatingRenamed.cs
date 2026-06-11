using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRatingRenamed : Change
    {
        public Guid RubricRatingId { get; set; }
        public string RatingTitle { get; set; }

        public RubricCriterionRatingRenamed(Guid rubricRatingId, string ratingTitle)
        {
            RubricRatingId = rubricRatingId;
            RatingTitle = ratingTitle;
        }
    }
}
