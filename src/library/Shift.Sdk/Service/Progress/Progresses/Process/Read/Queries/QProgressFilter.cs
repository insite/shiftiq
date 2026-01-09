using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QProgressFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public Guid? EventInstructorIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public Guid? StudentEmployerGroupIdentifier { get; set; }
        public Guid? StudentEmployerGroupStatusIdentifier { get; set; }
        public string GradebookTitle { get; set; }

        public DateTimeOffset? GradebookCreatedSince { get; set; }
        public DateTimeOffset? GradebookCreatedBefore { get; set; }

        public string ItemName { get; set; }

        public string ItemFormat { get; set; }

        public string[] ItemTypes { get; set; }

        public string ScoreText { get; set; }

        public string ProgressStatus { get; set; }

        public string ScoreComment { get; set; }

        public decimal? ScorePercentFrom { get; set; }

        public decimal? ScorePercentThru { get; set; }

        public string StudentName { get; set; }
        public Guid? StudentUserIdentifier { get; set; }
        public Guid? GradeItemIdentifier { get; set; }

        public Guid? UserPeriodIdentifier { get; set; }
        public DateTimeOffset? GradedSince { get; set; }
        public DateTimeOffset? GradedBefore { get; set; }
        public bool? IsScoreIgnored { get; set; }
    }

}
