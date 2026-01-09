using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCriterionRatingDescribed : Change
    {
        public Guid RubricRatingId { get; set; }
        public string RatingDescription { get; set; }

        public RubricCriterionRatingDescribed(Guid rubricRatingId, string ratingDescription)
        {
            RubricRatingId = rubricRatingId;
            RatingDescription = ratingDescription;
        }
    }
}
