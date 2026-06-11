using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class RenameRubricCriterionRating : Command, IHasRun
    {
        public Guid RubricRatingId { get; set; }
        public string RatingTitle { get; set; }

        public RenameRubricCriterionRating(Guid rubricId, Guid rubricRatingId, string ratingTitle)
        {
            AggregateIdentifier = rubricId;
            RubricRatingId = rubricRatingId;
            RatingTitle = ratingTitle;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (RatingTitle.IsEmpty())
                return true;

            var state = aggregate.Data;
            var rating = state.FindRating(RubricRatingId);
            if (rating == null || rating.Content.Title.Text.Default == RatingTitle)
                return true;

            aggregate.Apply(new RubricCriterionRatingRenamed(RubricRatingId, RatingTitle));

            return true;
        }
    }
}
