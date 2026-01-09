using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationIntegrationSettings : Command, IHasRun
    {
        public OrganizationIntegrations Integrations { get; set; }

        public ModifyOrganizationIntegrationSettings(Guid organizationId, OrganizationIntegrations integrations)
        {
            AggregateIdentifier = organizationId;
            Integrations = integrations;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var isBamboraSame = state.Integrations.Bambora.IsEqual(Integrations.Bambora);
            var isBCMailSame = state.Integrations.BCMail.IsEqual(Integrations.BCMail);
            var isRecaptchaSame = state.Integrations.Recaptcha.IsEqual(Integrations.Recaptcha);
            var isPrometricSame = state.Integrations.Prometric.IsEqual(Integrations.Prometric);
            var isScormCloudSame = state.Integrations.ScormCloud.IsEqual(Integrations.ScormCloud);

            if (isBamboraSame && isBCMailSame && isRecaptchaSame && isPrometricSame && isScormCloudSame)
                return true;

            if (isBamboraSame)
                Integrations.Bambora = null;

            if (isBCMailSame)
                Integrations.BCMail = null;

            if (isRecaptchaSame)
                Integrations.Recaptcha = null;

            if (isPrometricSame)
                Integrations.Prometric = null;

            if (isScormCloudSame)
                Integrations.ScormCloud = null;

            aggregate.Apply(new OrganizationIntegrationSettingsModified(Integrations));

            return true;
        }
    }
}
