using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QAchievementFilter : Filter
    {
        public List<Guid> OrganizationIdentifiers { get; set; } = new List<Guid>();
        public string AchievementTitle { get; set; }
        public List<string> AchievementLabels { get; set; } = new List<string>();
        public string AchievementDescription { get; set; }
        public bool? AchievementIsEnabled { get; set; }
        public string ExpirationType { get; set; }
        public int? ExpirationLifetimeQuantity { get; set; }
        public string ExpirationLifetimeUnit { get; set; }

        public DateTimeOffset? ExpirationFixedDateSince { get; set; }
        public DateTimeOffset? ExpirationFixedDateBefore { get; set; }

        public QAchievementFilter() { }

        public QAchievementFilter(Guid organizationId)
        {
            OrganizationIdentifiers.Add(organizationId);
        }

        public QAchievementFilter(Guid organizationId, string achievementType)
            : this(organizationId)
        {
            if (!string.IsNullOrEmpty(achievementType))
                AchievementLabels.Add(achievementType);
        }

        public QAchievementFilter Clone()
        {
            var clone = (QAchievementFilter)MemberwiseClone();

            clone.OrganizationIdentifiers = new List<Guid>(this.OrganizationIdentifiers);

            clone.AchievementLabels = new List<string>(this.AchievementLabels);

            return clone;
        }
    }
}