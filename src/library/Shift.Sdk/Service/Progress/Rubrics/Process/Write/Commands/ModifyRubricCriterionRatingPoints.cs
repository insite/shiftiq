using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    public class ModifyRubricCriterionRatingPoints : Command, IHasRun
    {
        public Guid RubricRatingId { get; set; }
        public decimal RatingPoints { get; set; }

        public ModifyRubricCriterionRatingPoints(Guid rubricId, Guid rubricRatingId, decimal ratingPoints)
        {
            AggregateIdentifier = rubricId;
            RubricRatingId = rubricRatingId;
            RatingPoints = ratingPoints;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            var state = aggregate.Data;
            var rating = state.FindRating(RubricRatingId);
            if (rating == null || rating.Points == RatingPoints)
                return true;

            aggregate.Apply(new RubricCriterionRatingPointsModified(RubricRatingId, RatingPoints));

            return true;
        }
    }
}
