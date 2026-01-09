using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class AddStandardOrganization : Command
    {
        public Guid[] OrganizationIds { get; set; }

        public AddStandardOrganization(Guid standardId, Guid organizationId)
            : this(standardId, new[] { organizationId })
        {
        }

        public AddStandardOrganization(Guid standardId, Guid[] organizationIds)
        {
            AggregateIdentifier = standardId;
            OrganizationIds = organizationIds;
        }
    }
}
