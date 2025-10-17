using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationParentModified : Change
    {
        public Guid? ParentOrganizationId { get; set; }

        public OrganizationParentModified(Guid? parentOrganizationId)
        {
            ParentOrganizationId = parentOrganizationId;
        }
    }
}
