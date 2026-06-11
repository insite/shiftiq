using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Activities
{
    [Obsolete]
    public class ActivityResourceChanged : Change
    {
        public Guid Resource { get; set; }

        public ActivityResourceChanged(Guid resource)
        {
            Resource = resource;
        }
    }
}
