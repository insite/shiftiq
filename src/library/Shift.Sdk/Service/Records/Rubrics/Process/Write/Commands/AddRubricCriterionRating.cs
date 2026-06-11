using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class AddRubricCriterionRating : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }
        public Guid RubricRatingId { get; set; }
        public string RatingTitle { get; set; }
        public decimal RatingPoints { get; set; }
        public int? RatingSequence { get; set; }

        public AddRubricCriterionRating(Guid rubricId, Guid rubricCriterionId, Guid rubricRatingId, string ratingTitle, decimal ratingPoints, int? ratingSequence)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            RubricRatingId = rubricRatingId;
            RatingTitle = ratingTitle;
            RatingPoints = ratingPoints;
            RatingSequence = ratingSequence;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (RatingTitle.IsEmpty())
                return true;

            var state = aggregate.Data;
            if (state.FindRating(RubricRatingId) != null)
                return true;

            var criterion = state.FindCriterion(RubricCriterionId);
            if (criterion == null)
                return true;

            aggregate.Apply(new RubricCriterionRatingAdded(RubricCriterionId, RubricRatingId, RatingTitle, RatingPoints, RatingSequence));

            return true;
        }
    }
}
