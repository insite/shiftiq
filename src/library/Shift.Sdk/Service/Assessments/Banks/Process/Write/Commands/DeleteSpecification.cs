using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteSpecification : Command
    {
        public Guid Specification { get; set; }

        public DeleteSpecification(Guid bank, Guid specification)
        {
            AggregateIdentifier = bank;
            Specification = specification;
        }
    }
}
