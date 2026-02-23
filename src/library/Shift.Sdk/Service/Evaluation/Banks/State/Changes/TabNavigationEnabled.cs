using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class TabNavigationEnabled : Change
    {
        public Guid Specification { get; set; }

        public TabNavigationEnabled(Guid specification)
        {
            Specification = specification;
        }
    }
}
