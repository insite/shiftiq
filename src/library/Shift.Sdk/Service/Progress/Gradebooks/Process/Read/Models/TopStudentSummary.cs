using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    public class TopStudentSummary
    {
        public class Student
        {
            public string FullName { get; set; }
            public string EmployerName { get; set; }
            public decimal Percent { get; set; }
        }

        public string EmployerRegion { get; set; }
        public string AchievementTitle { get; set; }
        public string ScoreItemName { get; set; }
        public IEnumerable<Student> Students { get; set; }
    }
}
