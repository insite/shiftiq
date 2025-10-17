using System;

namespace Shift.Contract
{
    public class CreateGradebook
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
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