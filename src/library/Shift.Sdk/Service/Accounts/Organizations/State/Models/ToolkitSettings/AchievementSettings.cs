using System;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AchievementSettings
    {
        public string DefaultAchievementType { get; set; }
        public string DefaultCertificateLayout { get; set; }
        public string CertificateFileNameTemplate { get; set; }
        public bool IsChangeNotificationEnabled { get; set; }
        public bool HideModulesInLearningSummary { get; set; }
        public bool ShowAchievementsInComplianceSummary { get; set; }

        public bool IsShallowEqual(AchievementSettings other)
        {
            return DefaultAchievementType.NullIfEmpty() == other.DefaultAchievementType.NullIfEmpty()
                && DefaultCertificateLayout.NullIfEmpty() == other.DefaultCertificateLayout.NullIfEmpty()
                && CertificateFileNameTemplate.NullIfEmpty() == other.CertificateFileNameTemplate.NullIfEmpty()
                && IsChangeNotificationEnabled == other.IsChangeNotificationEnabled
                && HideModulesInLearningSummary == other.HideModulesInLearningSummary
                && ShowAchievementsInComplianceSummary == other.ShowAchievementsInComplianceSummary
            ;
        }
    }
}
