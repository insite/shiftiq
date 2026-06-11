using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardOrganizationRemoved : Change
    {
        public Guid[] OrganizationIds { get; }

        public StandardOrganizationRemoved(Guid[] organizationIds)
        {
            OrganizationIds = organizationIds;
        }
    }
}
