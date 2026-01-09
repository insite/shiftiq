using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationDescription : Command, IHasRun
    {
        public CompanyDescription Description { get; set; }

        public ModifyOrganizationDescription(Guid organizationId, CompanyDescription description)
        {
            AggregateIdentifier = organizationId;
            Description = description;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.CompanyDescription.IsEqual(Description))
                return true;

            aggregate.Apply(new OrganizationDescriptionModified(Description));

            return true;
        }
    }
}
