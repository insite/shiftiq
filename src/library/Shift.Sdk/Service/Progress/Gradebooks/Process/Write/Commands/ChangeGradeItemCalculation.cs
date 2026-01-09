using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemCalculation : Command
    {
        public ChangeGradeItemCalculation(Guid record, Guid item, CalculationPart[] parts)
        {
            AggregateIdentifier = record;
            Item = item;
            Parts = parts;
        }

        public Guid Item { get; set; }
        public CalculationPart[] Parts { get; set; }
    }
}
