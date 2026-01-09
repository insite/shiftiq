using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationRegistrationSettings : Command, IHasRun
    {
        public UserRegistration Registration { get; set; }

        public ModifyOrganizationRegistrationSettings(Guid organizationId, UserRegistration registration)
        {
            AggregateIdentifier = organizationId;
            Registration = registration;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.UserRegistration.IsEqual(Registration))
                return true;

            aggregate.Apply(new OrganizationRegistrationSettingsModified(Registration));

            return true;
        }
    }
}
