using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationClosed : Change
    {
        public DateTimeOffset? Closed { get; set; }

        public OrganizationClosed(DateTimeOffset? closed)
        {
            Closed = closed;
        }
    }
}
