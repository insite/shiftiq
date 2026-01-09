using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    public class ModifyRubricCriterionIsRange : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }
        public bool IsRange { get; set; }

        public ModifyRubricCriterionIsRange(Guid rubricId, Guid rubricCriterionId, bool isRange)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            IsRange = isRange;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            var state = aggregate.Data;
            var criterion = state.FindCriterion(RubricCriterionId);
            if (criterion == null || criterion.IsRange == IsRange)
                return true;

            aggregate.Apply(new RubricCriterionIsRangeModified(RubricCriterionId, IsRange));

            return true;
        }
    }
}
