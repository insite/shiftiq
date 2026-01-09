using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAutomaticCompetencyExpirationModified : Change
    {
        public AutomaticCompetencyExpiration Settings { get; set; }

        public OrganizationAutomaticCompetencyExpirationModified(AutomaticCompetencyExpiration settings)
        {
            Settings = settings;
        }
    }
}
