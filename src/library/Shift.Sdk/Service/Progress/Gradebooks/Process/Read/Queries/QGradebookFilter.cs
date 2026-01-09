using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QGradebookFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public Guid? StandardIdentifier { get; set; }
        public Guid? PrimaryEventIdentifier { get; set; }
        public Guid? GradebookEventIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public Guid? EventInstructorIdentifier { get; set; }
        public string GradebookTitle { get; set; }
        public string[] GradebookTypes { get; set; }
        public DateTimeOffset? GradebookCreatedSince { get; set; }
        public DateTimeOffset? GradebookCreatedBefore { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsEventCancelled { get; set; }
        public Guid? StudentIdentifier { get; set; }
        public Guid? GradebookPeriodIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public string CourseName { get; set; }
    }
}