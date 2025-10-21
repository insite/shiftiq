using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardConfiguration : EntityTypeConfiguration<QStandard>
    {
        public QStandardConfiguration() : this("standard") { }

        public QStandardConfiguration(string schema)
        {
            ToTable("QStandard", schema);
            HasKey(x => new { x.StandardIdentifier });

            Property(x => x.AuthorName).IsUnicode(false).HasMaxLength(100);
            Property(x => x.Code).IsUnicode(false).HasMaxLength(256);
            Property(x => x.CompetencyScoreCalculationMethod).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CompetencyScoreSummarizationMethod).IsUnicode(false).HasMaxLength(50);
            Property(x => x.ContentName).IsUnicode(false).HasMaxLength(512);
            Property(x => x.ContentSettings).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CreditIdentifier).IsUnicode(false).HasMaxLength(128);
            Property(x => x.DocumentType).IsUnicode(false).HasMaxLength(40);
            Property(x => x.Icon).IsUnicode(false).HasMaxLength(32);
            Property(x => x.Language).IsUnicode(true).HasMaxLength(8);
            Property(x => x.LevelCode).IsUnicode(false).HasMaxLength(1);
            Property(x => x.LevelType).IsUnicode(false).HasMaxLength(20);
            Property(x => x.MajorVersion).IsUnicode(false).HasMaxLength(8);
            Property(x => x.MinorVersion).IsUnicode(false).HasMaxLength(8);
            Property(x => x.SourceDescriptor).IsUnicode(false).HasMaxLength(3400);
            Property(x => x.StandardLabel).IsUnicode(false).HasMaxLength(64);
            Property(x => x.StandardPrivacyScope).IsUnicode(false).HasMaxLength(10);
            Property(x => x.StandardTier).IsUnicode(false).HasMaxLength(30);
            Property(x => x.StandardType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.Tags).IsUnicode(false).HasMaxLength(100);
            Property(x => x.StandardAlias).IsUnicode(false).HasMaxLength(512);
            Property(x => x.StandardStatus).IsUnicode(false).HasMaxLength(32);
            Property(x => x.StandardHook).IsUnicode(false).HasMaxLength(9);
        }
    }
}
