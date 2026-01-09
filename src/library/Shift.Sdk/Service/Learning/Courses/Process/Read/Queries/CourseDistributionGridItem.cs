using System;
using System.ComponentModel;

namespace InSite.Application.Courses.Read
{
    public class CourseDistributionGridItem
    {
        public enum StatusType
        {
            [Description("None")]
            None,

            [Description("Unassigned")]
            Unassigned,

            [Description("Assigned")]
            Assigned,

            [Description("Not Started")]
            NotStarted,

            [Description("In Progress")]
            InProgress,

            [Description("Completed")]
            Completed
        }

        public Guid? LearnerUserIdentifier { get; set; }
        public string LearnerUserName { get; set; }
        public string LearnerUserEmail { get; set; }
        public Guid? FormIdentifier { get; set; }
        public string FormTitle { get; set; }
        public Guid? AttemptIdentifier { get; set; }
        public DateTimeOffset? AttemptImported { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptSubmitted { get; set; }
        public DateTimeOffset? AttemptGraded { get; set; }
        public decimal? AttemptScore { get; set; }
        public Guid CourseDistributionIdentifier { get; set; }
        public Guid ProductIdentifier { get; set; }
        public Guid? CourseIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid ManagerUserIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CourseEnrollmentIdentifier { get; set; }
        public DateTimeOffset DistributionAssigned { get; set; }
        public string DistributionStatus { get; set; }
        public DateTimeOffset? DistributionRedeemed { get; set; }
        public DateTimeOffset? DistributionExpiry { get; set; }
        public string DistributionComment { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }

        public StatusType GetStatus()
        {
            if (!LearnerUserIdentifier.HasValue)
                return StatusType.Unassigned;

            if (AttemptGraded.HasValue)
                return StatusType.Completed;

            if (AttemptStarted.HasValue || AttemptImported.HasValue)
                return StatusType.InProgress;

            if (CourseEnrollmentIdentifier.HasValue && !AttemptIdentifier.HasValue)
                return StatusType.NotStarted;

            if (CourseEnrollmentIdentifier.HasValue)
                return StatusType.Assigned;

            return StatusType.Unassigned;
        }
    }
}
