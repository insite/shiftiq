using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationPlatformUrl : Command, IHasRun
    {
        public PlatformUrl Url { get; set; }

        public ModifyOrganizationPlatformUrl(Guid organizationId, PlatformUrl url)
        {
            AggregateIdentifier = organizationId;
            Url = url;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.PlatformUrl.IsEqual(Url))
                return true;

            aggregate.Apply(new OrganizationPlatformUrlModified(Url));

            return true;
        }
    }
}
