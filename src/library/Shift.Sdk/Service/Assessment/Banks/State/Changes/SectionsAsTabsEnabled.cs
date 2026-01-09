using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SectionsAsTabsEnabled : Change
    {
        public Guid Specification { get; set; }

        public SectionsAsTabsEnabled(Guid specification)
        {
            Specification = specification;
        }
    }
}
