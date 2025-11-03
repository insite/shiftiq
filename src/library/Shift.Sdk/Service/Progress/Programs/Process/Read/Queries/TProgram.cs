using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TProgram
    {
        public Guid? CatalogIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid? NotificationCompletedAdministratorMessageIdentifier { get; set; }
        public Guid? NotificationCompletedLearnerMessageIdentifier { get; set; }
        public Guid? NotificationStalledAdministratorMessageIdentifier { get; set; }
        public Guid? NotificationStalledLearnerMessageIdentifier { get; set; }
        public Guid? CompletionTaskIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }

        public string AchievementElseCommand { get; set; }
        public string AchievementThenCommand { get; set; }
        public string AchievementWhenChange { get; set; }
        public string AchievementWhenGrade { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramTag { get; set; }
        public string ProgramType { get; set; }
        public string ProgramDescription { get; set; }
        public string ProgramName { get; set; }
        public string ProgramSlug { get; set; }
        public string ProgramIcon { get; set; }
        public string ProgramImage { get; set; }

        public int? CatalogSequence { get; set; }
        public int? NotificationStalledReminderLimit { get; set; }
        public int? NotificationStalledTriggerDay { get; set; }

        public bool IsHidden { get; set; }

        public DateTimeOffset? AchievementFixedDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual QAchievement Achievement { get; set; }

        public virtual ICollection<TTask> Tasks { get; set; } = new HashSet<TTask>();
        public virtual ICollection<TProgramEnrollment> Enrollments { get; set; } = new HashSet<TProgramEnrollment>();
        public virtual ICollection<TProgramGroupEnrollment> GroupEnrollments { get; set; } = new HashSet<TProgramGroupEnrollment>();
    }
}
