using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SpecificationDeleted : Change
    {
        public Guid Specification { get; set; }

        public SpecificationDeleted(Guid specification)
        {
            Specification = specification;
        }
    }
}
