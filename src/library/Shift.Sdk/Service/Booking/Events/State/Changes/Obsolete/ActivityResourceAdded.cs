using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Activities
{
    [Obsolete]
    public class ActivityResourceAdded : Change
    {
        public Guid Resource { get; set; }

        public ActivityResourceAdded(Guid resource)
        {
            Resource = resource;
        }
    }
}