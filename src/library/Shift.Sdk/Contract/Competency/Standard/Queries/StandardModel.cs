using System;

namespace Shift.Contract
{
    public partial class StandardModel
    {
        public Guid? BankId { get; set; }
        public Guid? BankSetId { get; set; }
        public Guid? CategoryItemId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? DepartmentGroupId { get; set; }
        public Guid? IndustryItemId { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? ParentStandardId { get; set; }
        public Guid StandardId { get; set; }
        public Guid? StandardStatusLastUpdateUser { get; set; }

        public bool IsCertificateEnabled { get; set; }
        public bool IsFeedbackEnabled { get; set; }
        public bool IsHidden { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPractical { get; set; }
        public bool IsPublished { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsTheory { get; set; }

        public string AuthorName { get; set; }
        public string CanvasIdentifier { get; set; }
        public string Code { get; set; }
        public string CompetencyScoreCalculationMethod { get; set; }
        public string CompetencyScoreSummarizationMethod { get; set; }
        public string ContentDescription { get; set; }
        public string ContentName { get; set; }
        public string ContentSettings { get; set; }
        public string ContentSummary { get; set; }
        public string ContentTitle { get; set; }
        public string CreditIdentifier { get; set; }
        public string DocumentType { get; set; }
        public string Icon { get; set; }
        public string Language { get; set; }
        public string LevelCode { get; set; }
        public string LevelType { get; set; }
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string SourceDescriptor { get; set; }
        public string StandardAlias { get; set; }
        public string StandardHook { get; set; }
        public string StandardLabel { get; set; }
        public string StandardPrivacyScope { get; set; }
        public string StandardStatus { get; set; }
        public string StandardTier { get; set; }
        public string StandardType { get; set; }
        public string Tags { get; set; }

        public int AssetNumber { get; set; }
        public int? CalculationArgument { get; set; }
        public int Sequence { get; set; }

        public decimal? CertificationHoursPercentCore { get; set; }
        public decimal? CertificationHoursPercentNonCore { get; set; }
        public decimal? CreditHours { get; set; }
        public decimal? MasteryPoints { get; set; }
        public decimal? PassingScore { get; set; }
        public decimal? PointsPossible { get; set; }

        public DateTimeOffset? AuthorDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? DatePosted { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTimeOffset? StandardStatusLastUpdateTime { get; set; }
        public DateTimeOffset? UtcPublished { get; set; }
    }
}