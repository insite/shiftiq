using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationSecretModified : Change
    {
        public string Secret { get; set; }

        public OrganizationSecretModified(string secret)
        {
            Secret = secret;
        }
    }
}
