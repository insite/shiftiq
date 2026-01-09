using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationParent : Command, IHasRun
    {
        public Guid? ParentOrganizationId { get; set; }

        public ModifyOrganizationParent(Guid organizationId, Guid? parentOrganizationId)
        {
            AggregateIdentifier = organizationId;
            ParentOrganizationId = parentOrganizationId;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.ParentOrganizationIdentifier == ParentOrganizationId)
                return true;

            aggregate.Apply(new OrganizationParentModified(ParentOrganizationId));

            return true;
        }
    }
}
