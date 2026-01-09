using System;

using Shift.Common.Timeline.Changes;


namespace InSite.Domain.Records
{
    public class GradeItemCalculationChanged : Change
    {
        public GradeItemCalculationChanged(Guid item, CalculationPart[] parts)
        {
            Item = item;
            Parts = parts;
        }

        public Guid Item { get; set; }
        public CalculationPart[] Parts { get; }
    }
}