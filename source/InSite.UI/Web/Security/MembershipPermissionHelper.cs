using System;

using InSite.Application.Contacts.Read;

namespace InSite.Web.Security
{
    static class MembershipPermissionHelper
    {
        public static bool CanModifyAdminMemberships()
        {
            return CurrentSessionState.Identity.Organization.Toolkits.Contacts?.EnableOperatorGroup != true
                || CurrentSessionState.Identity.IsOperator;
        }

        public static bool CanModifyMembership(Guid groupId)
        {
            if (CurrentSessionState.Identity.Organization.Toolkits.Contacts?.EnableOperatorGroup != true)
                return true;

            var group = ServiceLocator.GroupSearch.GetGroup(groupId);

            return group == null
                || !group.OnlyOperatorCanAddUser
                || CurrentSessionState.Identity.IsOperator;
        }

        public static bool CanModifyMembership(QGroup group)
        {
            if (CurrentSessionState.Identity.Organization.Toolkits.Contacts?.EnableOperatorGroup != true)
                return true;

            return group == null
                || !group.OnlyOperatorCanAddUser
                || CurrentSessionState.Identity.IsOperator;
        }
    }
}