using System;

namespace InSite.Persistence
{
    public class AchievementListGridItem
    {
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationName { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public string CategoryName { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string Visibility { get; set; }
    }
}
