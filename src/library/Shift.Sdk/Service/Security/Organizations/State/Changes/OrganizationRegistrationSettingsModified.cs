using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationRegistrationSettingsModified : Change
    {
        public UserRegistration Registration { get; set; }

        public OrganizationRegistrationSettingsModified(UserRegistration registration)
        {
            Registration = registration;
        }
    }
}
