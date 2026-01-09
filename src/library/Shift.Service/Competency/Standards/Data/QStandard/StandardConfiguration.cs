using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Competency;

public class StandardConfiguration : IEntityTypeConfiguration<StandardEntity>
{
    public void Configure(EntityTypeBuilder<StandardEntity> builder) 
    {
        builder.ToTable("QStandard", "standard");
        builder.HasKey(x => new { x.StandardIdentifier });
            
        builder.Property(x => x.AssetNumber).HasColumnName("AssetNumber").IsRequired();
        builder.Property(x => x.AuthorDate).HasColumnName("AuthorDate");
        builder.Property(x => x.AuthorName).HasColumnName("AuthorName").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.BankIdentifier).HasColumnName("BankIdentifier");
        builder.Property(x => x.BankSetIdentifier).HasColumnName("BankSetIdentifier");
        builder.Property(x => x.CalculationArgument).HasColumnName("CalculationArgument");
        builder.Property(x => x.CanvasIdentifier).HasColumnName("CanvasIdentifier").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CategoryItemIdentifier).HasColumnName("CategoryItemIdentifier");
        builder.Property(x => x.CertificationHoursPercentCore).HasColumnName("CertificationHoursPercentCore").HasPrecision(5, 2);
        builder.Property(x => x.CertificationHoursPercentNonCore).HasColumnName("CertificationHoursPercentNonCore").HasPrecision(5, 2);
        builder.Property(x => x.Code).HasColumnName("Code").IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.CompetencyScoreCalculationMethod).HasColumnName("CompetencyScoreCalculationMethod").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CompetencyScoreSummarizationMethod).HasColumnName("CompetencyScoreSummarizationMethod").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.ContentDescription).HasColumnName("ContentDescription").IsUnicode(true);
        builder.Property(x => x.ContentName).HasColumnName("ContentName").IsUnicode(false).HasMaxLength(512);
        builder.Property(x => x.ContentSettings).HasColumnName("ContentSettings").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ContentSummary).HasColumnName("ContentSummary").IsUnicode(true);
        builder.Property(x => x.ContentTitle).HasColumnName("ContentTitle").IsUnicode(false).HasMaxLength(1500);
        builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired();
        builder.Property(x => x.CreditHours).HasColumnName("CreditHours").HasPrecision(5, 2);
        builder.Property(x => x.CreditIdentifier).HasColumnName("CreditIdentifier").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.DatePosted).HasColumnName("DatePosted");
        builder.Property(x => x.DepartmentGroupIdentifier).HasColumnName("DepartmentGroupIdentifier");
        builder.Property(x => x.DocumentType).HasColumnName("DocumentType").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.Icon).HasColumnName("Icon").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.IndustryItemIdentifier).HasColumnName("IndustryItemIdentifier");
        builder.Property(x => x.IsCertificateEnabled).HasColumnName("IsCertificateEnabled").IsRequired();
        builder.Property(x => x.IsFeedbackEnabled).HasColumnName("IsFeedbackEnabled").IsRequired();
        builder.Property(x => x.IsHidden).HasColumnName("IsHidden").IsRequired();
        builder.Property(x => x.IsLocked).HasColumnName("IsLocked").IsRequired();
        builder.Property(x => x.IsPractical).HasColumnName("IsPractical").IsRequired();
        builder.Property(x => x.IsPublished).HasColumnName("IsPublished").IsRequired();
        builder.Property(x => x.IsTemplate).HasColumnName("IsTemplate").IsRequired();
        builder.Property(x => x.IsTheory).HasColumnName("IsTheory").IsRequired();
        builder.Property(x => x.Language).HasColumnName("Language").IsUnicode(true).HasMaxLength(8);
        builder.Property(x => x.LevelCode).HasColumnName("LevelCode").IsUnicode(false).HasMaxLength(1);
        builder.Property(x => x.LevelType).HasColumnName("LevelType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.MajorVersion).HasColumnName("MajorVersion").IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.MasteryPoints).HasColumnName("MasteryPoints").HasPrecision(5, 2);
        builder.Property(x => x.MinorVersion).HasColumnName("MinorVersion").IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.Modified).HasColumnName("Modified").IsRequired();
        builder.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.ParentStandardIdentifier).HasColumnName("ParentStandardIdentifier");
        builder.Property(x => x.PointsPossible).HasColumnName("PointsPossible").HasPrecision(5, 2);
        builder.Property(x => x.Sequence).HasColumnName("Sequence").IsRequired();
        builder.Property(x => x.SourceDescriptor).HasColumnName("SourceDescriptor").IsUnicode(false).HasMaxLength(3400);
        builder.Property(x => x.StandardAlias).HasColumnName("StandardAlias").IsUnicode(false).HasMaxLength(512);
        builder.Property(x => x.StandardIdentifier).HasColumnName("StandardIdentifier").IsRequired();
        builder.Property(x => x.StandardLabel).HasColumnName("StandardLabel").IsUnicode(false).HasMaxLength(64);
        builder.Property(x => x.StandardPrivacyScope).HasColumnName("StandardPrivacyScope").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.StandardStatus).HasColumnName("StandardStatus").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.StandardStatusLastUpdateTime).HasColumnName("StandardStatusLastUpdateTime");
        builder.Property(x => x.StandardStatusLastUpdateUser).HasColumnName("StandardStatusLastUpdateUser");
        builder.Property(x => x.StandardTier).HasColumnName("StandardTier").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.StandardType).HasColumnName("StandardType").IsRequired().IsUnicode(false).HasMaxLength(64);
        builder.Property(x => x.Tags).HasColumnName("Tags").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.UtcPublished).HasColumnName("UtcPublished");
        builder.Property(x => x.StandardHook).HasColumnName("StandardHook").IsUnicode(false).HasMaxLength(9);
        builder.Property(x => x.PassingScore).HasColumnName("PassingScore").HasPrecision(9, 8);

    }
}