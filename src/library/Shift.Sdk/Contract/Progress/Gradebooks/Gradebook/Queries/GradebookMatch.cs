using System;

namespace Shift.Contract
{
    public partial class GradebookMatch
    {
        public Guid GradebookId { get; set; }
        public string GradebookTitle { get; set; }
        public DateTimeOffset GradebookCreated { get; set; }
        public int GradebookEnrollmentCount { get; set; }

        public Guid? ClassId { get; set; }
        public string ClassTitle { get; set; }
        public DateTimeOffset? ClassStarted { get; set; }
        public DateTimeOffset? ClassEnded { get; set; }

        public Guid? AchievementId { get; set; }
        public string AchievementTitle { get; set; }
        public int AchievementCountGranted { get; set; }

        public bool IsLocked { get; set; }
    }
}