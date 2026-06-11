using System;
using System.Collections.Generic;

using InSite.Application.Records.Read;

namespace InSite.Application.Standards.Read
{
    public class QStandard
    {
        public int AssetNumber { get; set; }
        public DateTimeOffset? AuthorDate { get; set; }
        public string AuthorName { get; set; }
        public Guid? BankIdentifier { get; set; }
        public Guid? BankSetIdentifier { get; set; }
        public int? CalculationArgument { get; set; }
        public string CanvasIdentifier { get; set; }
        public Guid? CategoryItemIdentifier { get; set; }
        public decimal? CertificationHoursPercentCore { get; set; }
        public decimal? CertificationHoursPercentNonCore { get; set; }
        public string Code { get; set; }
        public string CompetencyScoreCalculationMethod { get; set; }
        public string CompetencyScoreSummarizationMethod { get; set; }
        public string ContentDescription { get; set; }
        public string ContentName { get; set; }
        public string ContentSettings { get; set; }
        public string ContentSummary { get; set; }
        public string ContentTitle { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public decimal? CreditHours { get; set; }
        public string CreditIdentifier { get; set; }
        public DateTimeOffset? DatePosted { get; set; }
        public Guid? DepartmentGroupIdentifier { get; set; }
        public string DocumentType { get; set; }
        public string Icon { get; set; }
        public Guid? IndustryItemIdentifier { get; set; }
        public bool IsCertificateEnabled { get; set; }
        public bool IsFeedbackEnabled { get; set; }
        public bool IsHidden { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPractical { get; set; }
        public bool IsPublished { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsTheory { get; set; }
        public string Language { get; set; }
        public string LevelCode { get; set; }
        public string LevelType { get; set; }
        public string MajorVersion { get; set; }
        public decimal? MasteryPoints { get; set; }
        public string MinorVersion { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ParentStandardIdentifier { get; set; }
        public decimal? PointsPossible { get; set; }
        public int Sequence { get; set; }
        public string SourceDescriptor { get; set; }
        public string StandardAlias { get; set; }
        public Guid StandardIdentifier { get; set; }
        public string StandardLabel { get; set; }
        public string StandardPrivacyScope { get; set; }
        public string StandardStatus { get; set; }
        public DateTimeOffset? StandardStatusLastUpdateTime { get; set; }
        public Guid? StandardStatusLastUpdateUser { get; set; }
        public string StandardTier { get; set; }
        public string StandardType { get; set; }
        public string Tags { get; set; }
        public DateTimeOffset? UtcPublished { get; set; }
        public string StandardHook { get; set; }
        public decimal? PassingScore { get; set; }

        public virtual ICollection<QAreaRequirement> JournalSetupAreaRequirements { get; set; } = new HashSet<QAreaRequirement>();

        public QStandard Clone()
        {
            return (QStandard)MemberwiseClone();
        }
    }
}
