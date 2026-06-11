using System;

namespace Shift.Contract
{
    public class CreateGradebook
    {
        public Guid? AchievementId { get; set; }
        public Guid? EventId { get; set; }
        public Guid? FrameworkId { get; set; }
        public Guid GradebookId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? PeriodId { get; set; }
        public bool IsLocked { get; set; }
        public string GradebookTitle { get; set; }
        public string GradebookType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string Reference { get; set; }
        public DateTimeOffset GradebookCreated { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }
    }
}