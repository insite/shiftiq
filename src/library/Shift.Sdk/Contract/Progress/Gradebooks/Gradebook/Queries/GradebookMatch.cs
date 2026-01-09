using System;

namespace Shift.Contract
{
    public partial class GradebookMatch
    {
        public Guid GradebookIdentifier { get; set; }
        public string GradebookTitle { get; set; }
        public DateTimeOffset GradebookCreated { get; set; }
        public int GradebookEnrollmentCount { get; set; }

        public Guid? ClassIdentifier { get; set; }
        public string ClassTitle { get; set; }
        public DateTimeOffset? ClassStarted { get; set; }
        public DateTimeOffset? ClassEnded { get; set; }

        public Guid? AchievementIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public int AchievementCountGranted { get; set; }

        public bool IsLocked { get; set; }
    }
}