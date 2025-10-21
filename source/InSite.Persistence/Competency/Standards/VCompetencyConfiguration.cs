using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    public class VCompetencyConfiguration : EntityTypeConfiguration<VCompetency>
    {
        public VCompetencyConfiguration() : this("standards") { }

        public VCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".VCompetency");
            HasKey(x => new { x.CompetencyIdentifier });

            Property(x => x.AreaAsset).IsOptional();
            Property(x => x.AreaCode).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.AreaIdentifier).IsOptional();
            Property(x => x.AreaLabel).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.AreaSize).IsOptional();
            Property(x => x.AreaTitle).IsOptional().IsUnicode(true);
            Property(x => x.CompetencyAsset).IsRequired();
            Property(x => x.CompetencyCode).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.CompetencyIdentifier).IsRequired();
            Property(x => x.CompetencyLabel).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.CompetencySize).IsOptional();
            Property(x => x.CompetencyTitle).IsOptional().IsUnicode(true);
            Property(x => x.FrameworkIdentifier).IsOptional();
        }
    }
}
