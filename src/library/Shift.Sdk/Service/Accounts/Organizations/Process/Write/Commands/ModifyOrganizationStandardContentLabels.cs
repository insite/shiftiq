using System;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationStandardContentLabels : Command, IHasRun
    {
        public string[] Labels { get; set; }

        public ModifyOrganizationStandardContentLabels(Guid organizationId, string[] labels)
        {
            AggregateIdentifier = organizationId;
            Labels = labels;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var existLabels = state.GetStandardContentLabels();
            var changeLabels = Labels.EmptyIfNull();
            var isSame = existLabels.Length == changeLabels.Length
                && existLabels.Zip(changeLabels, (a, b) => a == b).All(x => x);

            if (isSame)
                return true;

            aggregate.Apply(new OrganizationStandardContentLabelsModified(Labels));

            return true;
        }
    }
}
