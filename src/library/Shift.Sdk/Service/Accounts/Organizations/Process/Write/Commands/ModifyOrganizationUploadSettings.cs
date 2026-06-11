using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationUploadSettings : Command, IHasRun
    {
        public UploadSettings Settings { get; set; }

        public ModifyOrganizationUploadSettings(Guid organizationId, UploadSettings settings)
        {
            AggregateIdentifier = organizationId;
            Settings = settings;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.PlatformCustomization.UploadSettings.IsEqual(Settings))
                return true;

            aggregate.Apply(new OrganizationUploadSettingsModified(Settings));

            return true;
        }
    }
}
