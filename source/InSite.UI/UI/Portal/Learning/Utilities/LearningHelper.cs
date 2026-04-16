using InSite.Persistence;

namespace InSite.UI.Portal.Learning
{
    public class LearningHelper
    {
        public static bool ShowSafetyAchievementsOnly()
        {
            var roleNames = CurrentSessionState.Identity.GetRoleNames();
            var permissions = PermissionCache.Matrix.GetPermissions(CurrentSessionState.Identity.Organization.Code);
            var safetyAchievementsOnly = permissions.IsDenied("ui/home#safety-achievements-only", roleNames);

            return safetyAchievementsOnly;
        }
    }
}