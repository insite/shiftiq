using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGradebooks : Query<int>, IGradebookCriteria
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid? ClassIdentifier { get; set; }
        public Guid? ClassInstructorIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }

        public bool? IsLocked { get; set; }

        public string ClassTitle { get; set; }
        public string GradebookTitle { get; set; }
        public string GradebookType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string Reference { get; set; }

        public DateTimeOffset? ClassStartedSince { get; set; }
        public DateTimeOffset? ClassStartedBefore { get; set; }

        public DateTimeOffset? GradebookCreatedSince { get; set; }
        public DateTimeOffset? GradebookCreatedBefore { get; set; }

        public DateTimeOffset? LastChangeSince { get; set; }
        public DateTimeOffset? LastChangeBefore { get; set; }
    }
}