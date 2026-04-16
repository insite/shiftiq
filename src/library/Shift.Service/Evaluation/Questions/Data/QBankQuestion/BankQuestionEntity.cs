namespace Shift.Service.Assessment;

public class BankQuestionEntity
{
        public Guid BankIdentifier { get; set; }
        public Guid? CompetencyIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid? QuestionSourceIdentifier { get; set; }
        public Guid SetIdentifier { get; set; }
        public Guid? ParentQuestionIdentifier { get; set; }
        public Guid? RubricIdentifier { get; set; }

        public string? LastChangeType { get; set; }
        public string? LastChangeUser { get; set; }
        public string QuestionAssetNumber { get; set; } = default!;
        public string? QuestionCode { get; set; }
        public string? QuestionCondition { get; set; }
        public string? QuestionFlag { get; set; }
        public string? QuestionLikeItemGroup { get; set; }
        public string? QuestionReference { get; set; }
        public string? QuestionSourceAssetNumber { get; set; }
        public string? QuestionTag { get; set; }
        public string? QuestionTags { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public string? QuestionPublicationStatus { get; set; }
        public string? QuestionCalculationMethod { get; set; }
        public string? SetName { get; set; }
        public string? SubStandardIdentifiers { get; set; }
        public string? CreateUser { get; set; }

        public int BankIndex { get; set; }
        public int? BankSubIndex { get; set; }
        public int? QuestionDifficulty { get; set; }
        public int? QuestionTaxonomy { get; set; }

        public decimal? QuestionPoints { get; set; }
        public decimal? QuestionCutScore { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? QuestionFirstPublished { get; set; }
        public DateTimeOffset? CreateTime { get; set; }
}
