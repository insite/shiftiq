using System;

namespace InSite.Persistence
{
    public class MembershipFactory
    {
        public static Membership Create(Guid user, Guid group, Guid organization, string function = null, DateTimeOffset? effective = null)
        {
            return new Membership
            {
                GroupIdentifier = group,
                OrganizationIdentifier = organization,
                UserIdentifier = user,
                MembershipType = function,
                Assigned = effective ?? DateTimeOffset.UtcNow,
            };
        }
    }
}