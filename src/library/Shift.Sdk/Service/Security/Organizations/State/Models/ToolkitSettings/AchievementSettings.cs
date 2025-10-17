using System;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AchievementSettings
    {
        public string DefaultAchievementType { get; set; }
        public string DefaultCertificateLayout { get; set; }

        public bool IsShallowEqual(AchievementSettings other)
        {
            return DefaultAchievementType.NullIfEmpty() == other.DefaultAchievementType.NullIfEmpty()
                && DefaultCertificateLayout.NullIfEmpty() == other.DefaultCertificateLayout.NullIfEmpty();
        }
    }
}
