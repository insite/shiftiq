using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SpecificationRenamed : Change
    {
        public Guid Specification { get; set; }
        public string Name { get; set; }

        public SpecificationRenamed(Guid spec, string name)
        {
            Specification = spec;
            Name = name;
        }
    }
}
