using System;

namespace InSite.Persistence
{
    public class TAchievementDepartment
    {
        public Guid JoinIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public DateTimeOffset? Assigned { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
