using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QAchievementFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ParentOrganizationIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementLabel { get; set; }
        public string[] AchievementLabels { get; set; }
        public string AchievementDescription { get; set; }
        public bool? AchievementIsEnabled { get; set; }
        public string ExpirationType { get; set; }
        public int? ExpirationLifetimeQuantity { get; set; }
        public string ExpirationLifetimeUnit { get; set; }

        public DateTimeOffset? ExpirationFixedDateSince { get; set; }
        public DateTimeOffset? ExpirationFixedDateBefore { get; set; }

        public QAchievementFilter Clone()
        {
            return (QAchievementFilter)MemberwiseClone();
        }
    }
}