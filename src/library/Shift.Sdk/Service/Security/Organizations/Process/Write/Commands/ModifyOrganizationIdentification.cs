using System;

using InSite.Application.Organizations.Read;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationIdentification : Command, IHasRun, IHasValidation
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ModifyOrganizationIdentification(Guid organizationId, string code, string name)
        {
            AggregateIdentifier = organizationId;
            Code = code;
            Name = name;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;

            if (Code.IsEmpty())
                throw new AggregateException("The organization code is required property");

            if (Code.Length > 30)
                throw new AggregateException("The organization code value exceeded the maximum length.");

            if (Name.IsEmpty())
                throw new AggregateException("The company name is required property");

            var isSame = state.OrganizationCode == Code && state.CompanyName == Name;
            if (isSame)
                return true;

            aggregate.Apply(new OrganizationIdentificationModified(Code, Name));

            return true;
        }

        void IHasValidation.Validate(IOrganizationSearch search)
        {
            if (Code.IsNotEmpty() && search.CodeExists(Code, AggregateIdentifier))
                throw new AggregateException($"OrganizationCode is already assigned to another organization: {Code}");
        }
    }
}
