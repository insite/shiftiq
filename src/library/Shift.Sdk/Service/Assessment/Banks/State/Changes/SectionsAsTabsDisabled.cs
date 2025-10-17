using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SectionsAsTabsDisabled : Change
    {
        public Guid Specification { get; set; }

        public SectionsAsTabsDisabled(Guid specification)
        {
            Specification = specification;
        }
    }
}
