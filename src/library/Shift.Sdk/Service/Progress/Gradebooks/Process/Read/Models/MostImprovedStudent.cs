using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    public class MostImprovedStudent
    {
        public class Level
        {
            public string AchievementTitle { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public decimal Percent { get; set; }
        }

        public class Student
        {
            public string EmployerName { get; set; }
            public string EmployerRegion { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string AchievementDescription { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public decimal Difference { get; set; }

            public List<Level> Levels { get; set; }
        }

        public string AchievementDescription { get; set; }

        public List<Student> Students { get; set; }

        public int TotalStudentCount { get; set; }
    }
}
