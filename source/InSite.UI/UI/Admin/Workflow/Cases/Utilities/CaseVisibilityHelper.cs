using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Issues.Read;
using InSite.Domain.Foundations;
using InSite.Domain.Organizations;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.UI.Admin.Workflow.Cases.Utilities
{
    static class CaseVisibilityHelper
    {
        private static UserModel User => CurrentSessionState.Identity.User;
        private static OrganizationState Organization => CurrentSessionState.Identity.Organization;

        public static bool IsCaseVisible(VIssue issue)
        {
            var organizationId = Organization.Identifier;
            if (issue.OrganizationIdentifier != organizationId)
                return false;

            if (Organization.Toolkits.Issues?.DisplayOnlyConnectedCases != true || CurrentSessionState.Identity.IsAdministrator)
                return true;

            var userId = User.Identifier;
            if (issue.AdministratorUserIdentifier == userId || issue.OwnerUserIdentifier == userId || issue.TopicUserIdentifier == userId)
                return true;

            var filter1 = new QUserConnectionFilter
            {
                FromUserId = userId,
                ToUserOrganizationId = organizationId,
            };

            var filter2 = new QUserConnectionFilter
            {
                ToUserId = userId,
                FromUserOrganizationId = organizationId,
            };

            var toConnections = ServiceLocator.UserSearch.GetConnections(filter1);

            var hasConnections = toConnections.Count > 0 || ServiceLocator.UserSearch.CountConnections(filter2) > 0;

            return !hasConnections || toConnections.Find(x => x.ToUserIdentifier == issue.TopicUserIdentifier) != null;
        }
    }
}