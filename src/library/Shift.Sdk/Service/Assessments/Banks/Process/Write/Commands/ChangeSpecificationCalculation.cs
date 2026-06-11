using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeSpecificationCalculation : Command
    {
        public Guid Specification { get; set; }
        public ScoreCalculation Calculation { get; set; }

        public ChangeSpecificationCalculation(Guid bank, Guid spec, ScoreCalculation calculation)
        {
            AggregateIdentifier = bank;
            Specification = spec;            
            Calculation = calculation;
        }
    }
}