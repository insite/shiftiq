using System;

namespace InSite.Persistence
{
    public class VAchievementClassification
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid CategoryIdentifier { get; set; }
        public int? ClassificationSequence { get; set; }

        public virtual VAchievementCategory Category { get; set; }
    }
}
