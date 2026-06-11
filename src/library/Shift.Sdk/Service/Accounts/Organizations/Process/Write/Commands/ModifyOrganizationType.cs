using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationType : Command, IHasRun
    {
        public string Type { get; set; }

        public ModifyOrganizationType(Guid organizationId, string type)
        {
            AggregateIdentifier = organizationId;
            Type = type;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.OrganizationType.NullIfEmpty() == Type.NullIfEmpty())
                return true;

            aggregate.Apply(new OrganizationTypeModified(Type));

            return true;
        }
    }
}
