using System;

using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Controls.Talents.EmployeeCompetencies
{
    public static class CompetencyPositionHelper
    {
        public static CompetencyPosition GetPosition(CompetencyPositionParameter positionParameter)
        {
            var workingSet = GetWorkingSet(positionParameter);

            for (var i = 0; i < workingSet.Length; i++)
                if (workingSet[i] == positionParameter.CompetencyStandardIdentifier)
                {
                    var position = new CompetencyPosition
                    {
                        Count = workingSet.Length,
                        CurrentNumber = i + 1,
                        NextCompetencyStandardIdentifier = i == workingSet.Length - 1 ? Guid.Empty : workingSet[i + 1],
                        PrevCompetencyStandardIdentifier = i == 0 ? Guid.Empty : workingSet[i - 1]
                    };

                    return position;
                }

            return null;
        }

        public static void ClearWorkingSet(string searchRouteAction)
        {
            CurrentSessionState.SetCompetencyWorkingSet(searchRouteAction, null);
        }

        private static Guid[] LoadFromDatabase(CompetencyPositionParameter positionParameter)
        {
            var user = CurrentSessionState.Identity.User;
            var filter = LoadFilter(user.Identifier, positionParameter.SearchRouteAction);

            if (filter == null)
                return new Guid[] { };

            var validatorID = positionParameter.IsCompetenciesToValidate
                ? user.UserIdentifier
                : (Guid?)null;

            if (filter.UserIdentifier != positionParameter.UserIdentifier)
                return new Guid[] { };

            var identity = CurrentSessionState.Identity;
            var workersPermission = PermissionNames.Custom_CMDS_Workers;
            var parentUserID =
                identity.IsInRole(CmdsRole.Programmers)
                || identity.IsGranted(workersPermission, PermissionOperation.Delete)
                || identity.IsGranted(workersPermission, PermissionOperation.Configure)
                    ? (Guid?)null
                    : user.UserIdentifier;

            filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            filter.Paging = null;

            var table = UserCompetencyRepository.SelectSearchResults(filter, validatorID, parentUserID);

            var view = table.DefaultView;
            view.Sort = "Number";

            if (view.Count == 0)
                return new Guid[] { };

            var result = new Guid[view.Count];

            for (var i = 0; i < view.Count; i++)
                result[i] = (Guid)view[i]["CompetencyStandardIdentifier"];

            return result;
        }

        private static EmployeeCompetencyFilter LoadFilter(Guid userIdentifier, string routeName)
        {
            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == CurrentSessionState.Identity.Organization.Identifier
                  && x.UserIdentifier == userIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == routeName);

            var settings = ReportRequest.Deserialize(report?.ReportData)?.GetSearch<EmployeeCompetencyFilter>();

            return settings?.Filter;
        }

        private static Guid[] GetWorkingSet(CompetencyPositionParameter positionParameter)
        {
            var workingSet = CurrentSessionState.GetCompetencyWorkingSet(positionParameter.SearchRouteAction);

            if (workingSet == null)
            {
                workingSet = LoadFromDatabase(positionParameter);
                CurrentSessionState.SetCompetencyWorkingSet(positionParameter.SearchRouteAction, workingSet);
            }

            return workingSet;
        }
    }
}