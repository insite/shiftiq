using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationContactSettings : Command, IHasRun
    {
        public ContactSettings Contacts { get; set; }

        public ModifyOrganizationContactSettings(Guid organizationId, ContactSettings contacts)
        {
            AggregateIdentifier = organizationId;
            Contacts = contacts;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Contacts.IsEqual(Contacts))
                return true;

            aggregate.Apply(new OrganizationContactSettingsModified(Contacts));

            return true;
        }
    }
}
