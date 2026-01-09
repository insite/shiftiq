using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    public class OrganizationIntegrationSettingsModified : Change
    {
        public OrganizationIntegrations Integrations { get; set; }

        public OrganizationIntegrationSettingsModified(OrganizationIntegrations integrations)
        {
            Integrations = integrations;
        }
    }
}
