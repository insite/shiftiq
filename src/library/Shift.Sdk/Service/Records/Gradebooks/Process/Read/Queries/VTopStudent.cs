using System;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class VTopStudent
    {
        public Guid ProgressIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string AchievementTitle { get; set; }
        public string EmployerName { get; set; }
        public string EmployerRegion { get; set; }
        public string GradeItemName { get; set; }
        public decimal ProgressPercent { get; set; }
    }
}
