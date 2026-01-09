using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationUrls : Command, IHasRun
    {
        public OrganizationUrl Url { get; set; }

        public ModifyOrganizationUrls(Guid organizationId, OrganizationUrl url)
        {
            AggregateIdentifier = organizationId;
            Url = url;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.TenantUrl.IsEqual(Url))
                return true;

            aggregate.Apply(new OrganizationUrlsModified(Url));

            return true;
        }
    }
}
