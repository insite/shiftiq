using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class DescribeRubricCriterion : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }
        public string CriterionDescription { get; set; }

        public DescribeRubricCriterion(Guid rubricId, Guid rubricCriterionId, string criterionDescription)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            CriterionDescription = criterionDescription;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (CriterionDescription.IsEmpty())
                CriterionDescription = null;

            var state = aggregate.Data;
            var criterion = state.FindCriterion(RubricCriterionId);
            if (criterion == null || criterion.Content.Description.Text.Default == CriterionDescription)
                return true;

            aggregate.Apply(new RubricCriterionDescribed(RubricCriterionId, CriterionDescription));

            return true;
        }
    }
}
