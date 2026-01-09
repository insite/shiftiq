using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationCreated : Change
    {
        public DateTimeOffset? Opened { get; set; }

        public OrganizationCreated(DateTimeOffset? opened)
        {
            Opened = opened;
        }
    }
}
