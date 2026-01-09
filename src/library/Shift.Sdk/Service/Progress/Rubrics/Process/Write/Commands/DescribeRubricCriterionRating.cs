using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class DescribeRubricCriterionRating : Command, IHasRun
    {
        public Guid RubricRatingId { get; set; }
        public string RatingDescription { get; set; }

        public DescribeRubricCriterionRating(Guid rubricId, Guid rubricRatingId, string ratingDescription)
        {
            AggregateIdentifier = rubricId;
            RubricRatingId = rubricRatingId;
            RatingDescription = ratingDescription;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (RatingDescription.IsEmpty())
                RatingDescription = null;

            var state = aggregate.Data;
            var rating = state.FindRating(RubricRatingId);
            if (rating == null || rating.Content.Description.Text.Default == RatingDescription)
                return true;

            aggregate.Apply(new RubricCriterionRatingDescribed(RubricRatingId, RatingDescription));

            return true;
        }
    }
}
