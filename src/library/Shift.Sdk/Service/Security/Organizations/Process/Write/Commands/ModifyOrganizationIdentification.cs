using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationIdentification : Command, IHasRun
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }

        public ModifyOrganizationIdentification(Guid organizationId, string code, string name, string domain)
        {
            AggregateIdentifier = organizationId;
            Code = code;
            Name = name;
            Domain = domain;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var isSame = state.OrganizationCode.NullIfEmpty() == Code.NullIfEmpty()
                && state.CompanyName.NullIfEmpty() == Name.NullIfEmpty()
                && state.CompanyDomain.NullIfEmpty() == Domain.NullIfEmpty();

            if (isSame)
                return true;

            aggregate.Apply(new OrganizationIdentificationModified(Code, Name, Domain));

            return true;
        }
    }
}
