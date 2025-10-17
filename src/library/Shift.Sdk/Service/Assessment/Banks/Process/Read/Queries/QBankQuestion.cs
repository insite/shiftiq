using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;
using InSite.Application.Standards.Read;

namespace InSite.Application.Banks.Read
{
    public class QBankQuestion
    {
        public Guid BankIdentifier { get; set; }
        public Guid? CompetencyIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid? QuestionSourceIdentifier { get; set; }
        public Guid SetIdentifier { get; set; }
        public Guid? ParentQuestionIdentifier { get; set; }
        public Guid? RubricIdentifier { get; set; }

        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string QuestionAssetNumber { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionCondition { get; set; }
        public string QuestionFlag { get; set; }
        public string QuestionLikeItemGroup { get; set; }
        public string QuestionReference { get; set; }
        public string QuestionSourceAssetNumber { get; set; }
        public string QuestionTag { get; set; }
        public string QuestionTags { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }

        public int BankIndex { get; set; }
        public int? BankSubIndex { get; set; }
        public int? QuestionDifficulty { get; set; }
        public int? QuestionTaxonomy { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? QuestionFirstPublished { get; set; }

        public virtual QBank Bank { get; set; }
        public virtual VCompetency Competency { get; set; }
        public virtual QRubric Rubric { get; set; }

        public virtual ICollection<QBankQuestionSubCompetency> SubCompetencies { get; set; } = new HashSet<QBankQuestionSubCompetency>();
        public virtual ICollection<QBankQuestionGradeItem> GradeItems { get; set; } = new HashSet<QBankQuestionGradeItem>();
        public virtual ICollection<QBankOption> Options { get; set; } = new HashSet<QBankOption>();
    }
}
