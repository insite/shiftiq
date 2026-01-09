using System;

namespace InSite.Persistence
{
    public class TaskSearchItem
    {
        public Guid DepartmentIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public int? LifetimeMonths { get; set; }
        public bool IsRequired { get; set; }
        public bool IsPlanned { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
    }
}
