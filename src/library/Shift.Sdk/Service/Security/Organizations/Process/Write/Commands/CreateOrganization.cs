using System;

using InSite.Application.Organizations.Read;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Organizations.Write
{
    public class CreateOrganization : Command, IHasRun, IHasAggregate, IHasValidation
    {
        private OrganizationAggregate Aggregate { get; set; }

        OrganizationAggregate IHasAggregate.Aggregate => Aggregate;

        public string Code { get; set; }
        public string Name { get; set; }

        public DateTimeOffset? Opened { get; set; }

        public CreateOrganization(Guid organizationId, string code, string name, DateTimeOffset? opened = null)
        {
            AggregateIdentifier = organizationId;
            Code = code;
            Name = name;
            Opened = opened;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            if (Code.IsEmpty())
                throw new AggregateException("The organization code is required property");

            if (Code.Length > 30)
                throw new AggregateException("The organization code value exceeded the maximum length.");

            if (Name.IsEmpty())
                throw new AggregateException("The company name is required property");

            if (Name.Length > 50)
                throw new AggregateException("The company name value exceeded the maximum length.");

            Aggregate = new OrganizationAggregate { AggregateIdentifier = AggregateIdentifier };

            Aggregate.Apply(new OrganizationCreated(Opened, Code, Name));

            return true;
        }

        void IHasValidation.Validate(IOrganizationSearch search)
        {
            if (Code.IsNotEmpty() && search.CodeExists(Code))
                throw new AggregateException($"OrganizationCode is already assigned to another organization: {Code}");
        }
    }
}
