using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Persistence;
using InSite.Web.Security;

namespace InSite.Web.Data
{
    public static class MembershipHelper
    {
        public static Guid Save(Membership membership)
            => Save(membership, false, false);

        public static Guid Save(Membership membership, bool modifyFunction, bool modifyEffective, bool modifyExpiry = true)
        {
            return MembershipStore.Save(membership, modifyFunction, modifyEffective, modifyExpiry);
        }

        public static Membership Save(Guid group, Guid user, string membershipFunction)
        {
            var membership = new Membership
            {
                GroupIdentifier = group,
                UserIdentifier = user,
                MembershipType = membershipFunction,
                Assigned = DateTimeOffset.UtcNow
            };

            Save(membership, false, false);

            return membership;
        }

        public static void Save(Guid organizationId, string groupType, string groupName, Guid user, string membershipFunction)
        {
            var filter = new QGroupFilter
            {
                OrganizationIdentifier = organizationId,
                GroupName = groupName,
                GroupType = groupType
            };

            var group = ServiceLocator.GroupSearch.GetGroups(filter).FirstOrDefault();
            if (group == null || !MembershipPermissionHelper.CanModifyMembership(group))
                return;

            var membership = MembershipFactory.Create(user, group.GroupIdentifier, organizationId, membershipFunction);

            MembershipStore.Save(membership);
        }
    }
}