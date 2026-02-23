using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class AddCriterion : Command
    {
        public Guid Identifier { get; set; }
        public Guid Specification { get; set; }
        public Guid[] Sets { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public int QuestionLimit { get; set; }
        public string BasicFilter { get; set; }
        public PivotTable AdvancedFilter { get; set; }

        public AddCriterion(Guid bank, Guid spec, Guid[] sets, Guid criterion, string name, decimal weight, int questionLimit, string basicFilter, PivotTable advancedFilter)
        {
            AggregateIdentifier = bank;

            Specification = spec;
            Sets = sets;
            Identifier = criterion;
            Name = name;
            Weight = weight;
            QuestionLimit = questionLimit;
            BasicFilter = basicFilter;
            AdvancedFilter = advancedFilter;
        }
    }
}