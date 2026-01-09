using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationAdministrator : Command, IHasRun
    {
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }

        public ModifyOrganizationAdministrator(Guid organizationId, Guid? userId, Guid? groupId)
        {
            AggregateIdentifier = organizationId;
            UserId = userId;
            GroupId = groupId;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.AdministratorUserIdentifier == UserId && state.AdministratorGroupIdentifier == GroupId)
                return true;

            aggregate.Apply(new OrganizationAdministratorModified(UserId, GroupId));

            return true;
        }
    }
}
