using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationGlossaryModified : Change
    {
        public Guid? GlossaryId { get; set; }

        public OrganizationGlossaryModified(Guid? glossaryId)
        {
            GlossaryId = glossaryId;
        }
    }
}
