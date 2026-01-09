using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SpecificationCalculationChanged : Change
    {
        public Guid Specification { get; set; }
        public ScoreCalculation Calculation { get; set; }

        public SpecificationCalculationChanged(Guid spec, ScoreCalculation calculation)
        {
            Specification = spec;
            Calculation = calculation;
        }
    }
}
