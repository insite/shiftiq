using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class RetypeSpecification : Command
    {
        public Guid Specification { get; set; }
        public SpecificationType Type { get; set; }

        public RetypeSpecification(Guid bank, Guid spec, SpecificationType type)
        {
            AggregateIdentifier = bank;
            Specification = spec;
            Type = type;
        }
    }
}