using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class RenameRubricCriterion : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }
        public string CriterionTitle { get; set; }

        public RenameRubricCriterion(Guid rubricId, Guid rubricCriterionId, string criterionTitle)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            CriterionTitle = criterionTitle;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (CriterionTitle.IsEmpty())
                return true;

            var state = aggregate.Data;
            var criterion = state.FindCriterion(RubricCriterionId);
            if (criterion == null || criterion.Content.Title.Text.Default == CriterionTitle)
                return true;

            aggregate.Apply(new RubricCriterionRenamed(RubricCriterionId, CriterionTitle));

            return true;
        }
    }
}
