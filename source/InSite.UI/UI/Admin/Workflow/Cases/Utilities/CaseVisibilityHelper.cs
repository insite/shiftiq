using System;

using InSite.Application.Contacts.Read;
using InSite.Domain.Foundations;
using InSite.Domain.Organizations;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.UI.Admin.Workflow.Cases.Utilities
{
    static class CaseVisibilityHelper
    {
        private static UserModel User => CurrentSessionState.Identity.User;
        private static OrganizationState Organization => CurrentSessionState.Identity.Organization;

        public static bool IsCaseVisible(Guid caseOrganizationId, Guid? topicUserId)
        {
            if (caseOrganizationId != Organization.Identifier)
                return false;

            if (Organization.Toolkits.Issues?.DisplayOnlyConnectedCases != true || topicUserId == User.Identifier)
                return true;

            var filter1 = new QUserConnectionFilter
            {
                FromUserId = User.Identifier,
                ToUserOrganizationId = Organization.Identifier,
            };

            var filter2 = new QUserConnectionFilter
            {
                ToUserId = User.Identifier,
                FromUserOrganizationId = Organization.Identifier,
            };

            var toConnections = ServiceLocator.UserSearch.GetConnections(filter1);

            var hasConnections = toConnections.Count > 0 || ServiceLocator.UserSearch.CountConnections(filter2) > 0;

            return !hasConnections || toConnections.Find(x => x.ToUserIdentifier == topicUserId) != null;
        }
    }
}