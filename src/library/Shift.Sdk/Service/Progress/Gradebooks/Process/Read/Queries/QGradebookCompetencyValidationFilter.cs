using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QGradebookCompetencyValidationFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public Guid? EventInstructorIdentifier { get; set; }
        public Guid? StudentEmployerGroupIdentifier { get; set; }
        public string GradebookTitle { get; set; }
        public DateTimeOffset? GradebookCreatedSince { get; set; }
        public DateTimeOffset? GradebookCreatedBefore { get; set; }
        public decimal? PointsFrom { get; set; }
        public decimal? PointsThru { get; set; }
        public string StudentName { get; set; }
        public Guid? CompetencyIdentifier { get; set; }
        public bool NotAchievedMastery { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? GradebookPeriodIdentifier { get; set; }
        public Guid? UserPeriodIdentifier { get; set; }
    }
}