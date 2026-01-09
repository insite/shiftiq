using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class TabNavigationDisabled : Change
    {
        public Guid Specification { get; set; }

        public TabNavigationDisabled(Guid specification)
        {
            Specification = specification;
        }
    }
}
