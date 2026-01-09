using System.Linq;
using System.Web;

using InSite.Persistence;

namespace InSite
{
    public static class RecentSessionHelper
    {
        private static TUserSessionCacheSummary Entity
        {
            get => (TUserSessionCacheSummary)HttpContext.Current.Session[typeof(RecentSessionHelper) + "." + nameof(Entity)];
            set => HttpContext.Current.Session[typeof(RecentSessionHelper) + "." + nameof(Entity)] = value;
        }

        private static bool IsLoaded
        {
            get => (bool?)HttpContext.Current.Session[typeof(RecentSessionHelper) + "." + nameof(IsLoaded)] ?? false;
            set => HttpContext.Current.Session[typeof(RecentSessionHelper) + "." + nameof(IsLoaded)] = value;
        }

        public static TUserSessionCacheSummary Get()
        {
            if (!IsLoaded)
            {
                var accessibleOrganizations = OrganizationHelper.SelectOrganizationsAccessibleToUser(CurrentSessionState.Identity.User.UserIdentifier);

                Entity = OrganizationHelper.GetRecentSession(
                    CurrentSessionState.Identity.User.UserIdentifier, 
                    accessibleOrganizations.Select(x => x.OrganizationCode).ToArray(), 
                    new[] { CurrentSessionState.Identity.Organization.Code });

                IsLoaded = true;
            }

            return Entity;
        }

        public static void Clear()
        {
            Entity = null;
            IsLoaded = false;
        }
    }
}