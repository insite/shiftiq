using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    public class VStandardConfiguration : EntityTypeConfiguration<VStandard>
    {
        public VStandardConfiguration() : this("standards") { }

        public VStandardConfiguration(string schema)
        {
            ToTable(schema + ".VStandard");
            HasKey(x => new { x.StandardIdentifier });

            Property(x => x.ParentStandardIdentifier).IsOptional();
            Property(x => x.StandardAsset).IsOptional();
            Property(x => x.StandardCode).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.StandardIdentifier).IsRequired();
            Property(x => x.StandardLabel).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.StandardTitle).IsOptional().IsUnicode(true);
            Property(x => x.StandardType).IsRequired().IsUnicode(false).HasMaxLength(64);

            Property(x => x.CompetencyScoreSummarizationMethod).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CompetencyScoreCalculationMethod).IsOptional().IsUnicode(false).HasMaxLength(50);

            HasOptional(a => a.Parent).WithMany(b => b.Children).HasForeignKey(c => c.ParentStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
