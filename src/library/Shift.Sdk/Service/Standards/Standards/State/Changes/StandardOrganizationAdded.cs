using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardOrganizationAdded : Change
    {
        public Guid[] OrganizationIds { get; }

        public StandardOrganizationAdded(Guid[] organizationIds)
        {
            OrganizationIds = organizationIds;
        }
    }
}
