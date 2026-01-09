using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class VAchievementCategory
    {
        public Guid CategoryIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string AchievementLabel { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        public ICollection<VAchievementClassification> Classifications { get; set; }
    }
}
