using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandardOrganization : Command
    {
        public Guid[] OrganizationIds { get; set; }

        public RemoveStandardOrganization(Guid standardId, Guid organizationId)
            : this(standardId, new[] { organizationId })
        {
        }

        public RemoveStandardOrganization(Guid standardId, Guid[] organizationIds)
        {
            AggregateIdentifier = standardId;
            OrganizationIds = organizationIds;
        }

    }
}
