using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QExperience
    {
        public Guid ExperienceIdentifier { get; set; }
        public Guid JournalIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }
        public int Sequence { get; set; }
        public string Employer { get; set; }
        public string Supervisor { get; set; }
        public string Instructor { get; set; }
        public DateTimeOffset ExperienceCreated { get; set; }
        public DateTime? ExperienceStarted { get; set; }
        public DateTime? ExperienceStopped { get; set; }
        public DateTime? ExperienceCompleted { get; set; }
        public DateTimeOffset? ExperienceValidated { get; set; }
        public decimal? ExperienceHours { get; set; }
        public string ExperienceEvidence { get; set; }
        public string TrainingLevel { get; set; }
        public string TrainingLocation { get; set; }
        public string TrainingProvider { get; set; }
        public string TrainingCourseTitle { get; set; }
        public string TrainingComment { get; set; }
        public string TrainingType { get; set; }
        public int? SkillRating { get; set; }
        public string MediaEvidenceName { get; set; }
        public string MediaEvidenceType { get; set; }
        public Guid? MediaEvidenceFileIdentifier { get; set; }

        public virtual QJournal Journal { get; set; }
        public virtual VUser Validator { get; set; }

        public virtual ICollection<QExperienceCompetency> Competencies { get; set; } = new HashSet<QExperienceCompetency>();
    }
}
