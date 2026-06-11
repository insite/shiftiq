using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAdministratorModified : Change
    {
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }

        public OrganizationAdministratorModified(Guid? userId, Guid? groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }
    }
}
