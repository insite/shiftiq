using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationGradebookSettings : Command, IHasRun
    {
        public GradebookSettings Gradebooks { get; set; }

        public ModifyOrganizationGradebookSettings(Guid organizationId, GradebookSettings gradebooks)
        {
            AggregateIdentifier = organizationId;
            Gradebooks = gradebooks;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Gradebooks.IsEqual(Gradebooks))
                return true;

            aggregate.Apply(new OrganizationGradebookSettingsModified(Gradebooks));

            return true;
        }
    }
}
