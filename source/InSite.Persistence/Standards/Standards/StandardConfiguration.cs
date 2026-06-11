using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardConfiguration : EntityTypeConfiguration<Standard>
    {
        public StandardConfiguration() : this("standards") { }

        public StandardConfiguration(string schema)
        {
            ToTable(schema + ".Standard");
            HasKey(x => new { x.StandardIdentifier });

            Property(x => x.AssetNumber).IsRequired();
            Property(x => x.AuthorDate).IsOptional();
            Property(x => x.AuthorName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CalculationArgument).IsOptional();
            Property(x => x.CertificationHoursPercentCore).IsOptional();
            Property(x => x.CertificationHoursPercentNonCore).IsOptional();
            Property(x => x.Code).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.CompetencyScoreCalculationMethod).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CompetencyScoreSummarizationMethod).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.ContentDescription).IsOptional().IsUnicode(true);
            Property(x => x.ContentName).IsOptional().IsUnicode(false).HasMaxLength(512);
            Property(x => x.ContentSettings).IsOptional().IsUnicode(false);
            Property(x => x.ContentSummary).IsOptional().IsUnicode(true);
            Property(x => x.ContentTitle).IsOptional().IsUnicode(true);
            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.CreditHours).IsOptional();
            Property(x => x.CreditIdentifier).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.DatePosted).IsOptional();
            Property(x => x.DocumentType).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.StandardHook).IsOptional().IsUnicode(false).HasMaxLength(9);
            Property(x => x.Icon).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.IsCertificateEnabled).IsRequired();
            Property(x => x.IsFeedbackEnabled).IsRequired();
            Property(x => x.IsHidden).IsRequired();
            Property(x => x.IsLocked).IsRequired();
            Property(x => x.IsPractical).IsRequired();
            Property(x => x.IsPublished).IsRequired();
            Property(x => x.IsTemplate).IsRequired();
            Property(x => x.IsTheory).IsRequired();
            Property(x => x.Language).IsOptional().IsUnicode(true).HasMaxLength(8);
            Property(x => x.LevelCode).IsOptional().IsUnicode(false).HasMaxLength(1);
            Property(x => x.LevelType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.MajorVersion).IsOptional().IsUnicode(false).HasMaxLength(8);
            Property(x => x.MasteryPoints).IsOptional();
            Property(x => x.MinorVersion).IsOptional().IsUnicode(false).HasMaxLength(8);
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.ParentStandardIdentifier).IsOptional();
            Property(x => x.PointsPossible).IsOptional();
            Property(x => x.Sequence).IsRequired();
            Property(x => x.SourceDescriptor).IsOptional().IsUnicode(false);
            Property(x => x.StandardIdentifier).IsRequired();
            Property(x => x.StandardLabel).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.StandardPrivacyScope).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.StandardTier).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.StandardType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.Tags).IsOptional().IsUnicode(false);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UtcPublished).IsOptional();
            Property(x => x.StandardAlias).IsOptional().IsUnicode(false).HasMaxLength(512);
            Property(x => x.DepartmentGroupIdentifier).IsOptional();
            Property(x => x.IndustryItemIdentifier).IsOptional();
            Property(x => x.StandardStatus).IsUnicode(false).HasMaxLength(32);
        }
    }
}