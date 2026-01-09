using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationSurveySettings : Command, IHasRun
    {
        public SurveySettings Surveys { get; set; }

        public ModifyOrganizationSurveySettings(Guid organizationId, SurveySettings surveys)
        {
            AggregateIdentifier = organizationId;
            Surveys = surveys;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            if (state.Toolkits.Surveys.IsEqual(Surveys))
                return true;

            aggregate.Apply(new OrganizationSurveySettingsModified(Surveys));

            return true;
        }
    }
}
