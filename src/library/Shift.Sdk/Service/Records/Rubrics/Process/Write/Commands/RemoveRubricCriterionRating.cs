using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    public class RemoveRubricCriterionRating : Command, IHasRun
    {
        public Guid RubricRatingId { get; set; }

        public RemoveRubricCriterionRating(Guid rubricId, Guid rubricRatingId)
        {
            AggregateIdentifier = rubricId;
            RubricRatingId = rubricRatingId;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.FindRating(RubricRatingId) == null)
                return true;

            aggregate.Apply(new RubricCriterionRatingRemoved(RubricRatingId));

            return true;
        }
    }
}
