using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class TAchievementDepartment
    {
        public Guid JoinIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public DateTimeOffset? Assigned { get; set; }
        public DateTimeOffset? Modified { get; set; }

        public virtual QAchievement Achievement { get; set; }
        public virtual QGroup Department { get; set; }
    }
}
