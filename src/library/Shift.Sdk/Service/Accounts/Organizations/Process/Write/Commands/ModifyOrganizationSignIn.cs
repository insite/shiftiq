using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationSignIn : Command, IHasRun
    {
        public PlatformCustomizationSignIn Settings { get; set; }

        public ModifyOrganizationSignIn(Guid organizationId, PlatformCustomizationSignIn settings)
        {
            AggregateIdentifier = organizationId;
            Settings = settings;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.SignIn.IsEqual(Settings))
                return true;

            aggregate.Apply(new OrganizationSignInModified(Settings));

            return true;
        }
    }
}
