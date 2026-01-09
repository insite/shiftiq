using System;
using System.Collections.Generic;

using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;

namespace InSite.Application.Standards.Read
{
    public class VCompetency
    {
        public Guid? AreaIdentifier { get; set; }
        public Guid CompetencyIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }

        public string AreaCode { get; set; }
        public string AreaLabel { get; set; }
        public string AreaStandardType { get; set; }
        public string AreaTitle { get; set; }
        public string CompetencyCode { get; set; }
        public string CompetencyLabel { get; set; }
        public string CompetencyTitle { get; set; }

        public int? AreaAsset { get; set; }
        public int? AreaSize { get; set; }
        public int? AreaSequence { get; set; }
        public int CompetencyAsset { get; set; }
        public int? CompetencySize { get; set; }
        public int CompetencySequence { get; set; }

        public virtual ICollection<QBankQuestion> Questions { get; set; } = new HashSet<QBankQuestion>();
        public virtual ICollection<QBankOption> Options { get; set; } = new HashSet<QBankOption>();
        public virtual ICollection<QCompetencyRequirement> CompetencyRequirements { get; set; } = new HashSet<QCompetencyRequirement>();
        public virtual ICollection<QExperienceCompetency> ExperienceCompetencies { get; set; } = new HashSet<QExperienceCompetency>();
        public virtual ICollection<QBankQuestionSubCompetency> QuestionSubCompetencies { get; set; } = new HashSet<QBankQuestionSubCompetency>();
    }
}
