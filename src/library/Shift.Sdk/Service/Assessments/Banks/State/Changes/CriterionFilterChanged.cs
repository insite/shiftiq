using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Banks
{
    public class CriterionFilterChanged : Change
    {
        public Guid Criterion { get; set; }
        public decimal SetWeight { get; set; }
        public int? QuestionLimit { get; set; }
        public string TagFilter { get; set; }
        public PivotTable PivotFilter { get; set; }

        public CriterionFilterChanged(Guid sieve, decimal setWeight, int? questionLimit, string filter, PivotTable pivot)
        {
            Criterion = sieve;
            SetWeight = setWeight;
            QuestionLimit = questionLimit;
            TagFilter = filter;
            PivotFilter = pivot;
        }
    }
}
