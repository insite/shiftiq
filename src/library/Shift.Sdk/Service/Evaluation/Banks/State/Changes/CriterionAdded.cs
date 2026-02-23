using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Banks
{
    public class CriterionAdded : Change
    {
        public Guid Identifier { get; set; }
        public Guid Specification { get; set; }
        public Guid[] Sets { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public int QuestionLimit { get; set; }
        public string TagFilter { get; set; }
        public PivotTable PivotFilter { get; set; }

        public CriterionAdded(Guid spec, Guid[] sets, Guid sieve, string name, decimal weight, int questionLimit, string tagFilter, PivotTable pivotFilter)
        {
            Specification = spec;
            Sets = sets;
            Identifier = sieve;
            Name = name;
            Weight = weight;
            QuestionLimit = questionLimit;
            TagFilter = tagFilter;
            PivotFilter = pivotFilter;
        }
    }
}