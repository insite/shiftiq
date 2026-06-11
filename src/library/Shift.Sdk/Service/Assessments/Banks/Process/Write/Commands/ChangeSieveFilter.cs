using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeCriterionFilter : Command
    {
        public Guid Criterion { get; set; }
        public decimal SetWeight { get; set; }
        public int? QuestionLimit { get; set; }
        public string TagFilter { get; set; }
        public PivotTable PivotFilter { get; set; }

        public ChangeCriterionFilter(Guid bank, Guid criterion, decimal setWeight, int? questionLimit, string tagFilter, PivotTable pivotFilter)
        {
            AggregateIdentifier = bank;
            Criterion = criterion;
            SetWeight = setWeight;
            QuestionLimit = questionLimit;
            TagFilter = tagFilter;
            PivotFilter = pivotFilter;
        }
    }
}
