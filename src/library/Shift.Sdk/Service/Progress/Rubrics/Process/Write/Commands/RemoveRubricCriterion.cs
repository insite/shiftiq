using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    public class RemoveRubricCriterion : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }

        public RemoveRubricCriterion(Guid rubricId, Guid rubricCriterionId)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.FindCriterion(RubricCriterionId) == null)
                return true;

            aggregate.Apply(new RubricCriterionRemoved(RubricCriterionId));

            return true;
        }
    }
}
