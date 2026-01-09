using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationSecret : Command, IHasRun
    {
        public string Secret { get; set; }

        public ModifyOrganizationSecret(Guid organizationId, string secret)
        {
            AggregateIdentifier = organizationId;
            Secret = secret;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.OrganizationSecret.NullIfEmpty() == Secret.NullIfEmpty())
                return true;

            aggregate.Apply(new OrganizationSecretModified(Secret));

            return true;
        }
    }
}
