using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationSignInModified : Change
    {
        public PlatformCustomizationSignIn Settings { get; set; }

        public OrganizationSignInModified(PlatformCustomizationSignIn settings)
        {
            Settings = settings;
        }
    }
}
