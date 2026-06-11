using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class AddRubricCriterion : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }
        public string CriterionTitle { get; set; }
        public bool IsRange { get; set; }
        public int? CriterionSequence { get; set; }

        public AddRubricCriterion(Guid rubricId, Guid rubricCriterionId, string criterionTitle, bool isRange, int? criterionSequence = null)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            CriterionTitle = criterionTitle;
            IsRange = isRange;
            CriterionSequence = criterionSequence;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.FindCriterion(RubricCriterionId) != null || CriterionTitle.IsEmpty())
                return true;

            aggregate.Apply(new RubricCriterionAdded(RubricCriterionId, CriterionTitle, IsRange, CriterionSequence));

            return true;
        }
    }
}
