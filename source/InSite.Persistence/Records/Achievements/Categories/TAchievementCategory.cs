using System;

namespace InSite.Persistence
{
    public class TAchievementCategory
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid ItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public int? CategorySequence { get; set; }

        public string CategoryDescription { get; set; }

        public virtual TCollectionItem Category { get; set; }
    }
}
