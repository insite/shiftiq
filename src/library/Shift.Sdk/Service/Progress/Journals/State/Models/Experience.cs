using System;
using System.Collections.Generic;

namespace InSite.Domain.Records
{
    [Serializable]
    public class Experience
    {
        public Guid Identifier { get; set; }
        public Guid? Validator { get; set; }
        public int Sequence { get; set; }
        public string Employer { get; set; }
        public string Supervisor { get; set; }
        public string Instructor { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Stopped { get; set; }
        public DateTime? Completed { get; set; }
        public decimal? Hours { get; set; }
        public string Evidence { get; set; }
        public string TrainingLevel { get; set; }
        public string TrainingLocation { get; set; }
        public string TrainingProvider { get; set; }
        public string TrainingCourseTitle { get; set; }
        public string TrainingComment { get; set; }
        public string TrainingType { get; set; }
        public int? SkillRating { get; set; }
        public DateTimeOffset? Validated { get; set; }
        public string MediaEvidenceName { get; set; }
        public string MediaEvidenceType { get; set; }
        public Guid? MediaEvidenceFileIdentifier { get; set; }

        public List<ExperienceCompetency> Competencies { get; set; } = new List<ExperienceCompetency>();

        public ExperienceCompetency FindExperienceCompetency(Guid competency)
            => Competencies.Find(x => x.Competency == competency);

        public bool ShouldSerializeCompetencies() => Competencies.Count > 0;
    }
}
