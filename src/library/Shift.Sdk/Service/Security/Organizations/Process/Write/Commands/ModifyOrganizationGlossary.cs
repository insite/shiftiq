using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationGlossary : Command, IHasRun
    {
        public Guid? GlossaryId { get; set; }

        public ModifyOrganizationGlossary(Guid organizationId, Guid? glossaryId)
        {
            AggregateIdentifier = organizationId;
            GlossaryId = glossaryId;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (GlossaryId == Guid.Empty || state.GlossaryIdentifier == GlossaryId)
                return true;

            aggregate.Apply(new OrganizationGlossaryModified(GlossaryId));

            return true;
        }
    }
}
