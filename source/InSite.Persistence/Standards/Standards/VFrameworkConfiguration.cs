using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    public class VFrameworkConfiguration : EntityTypeConfiguration<VFramework>
    {
        public VFrameworkConfiguration() : this("standards") { }

        public VFrameworkConfiguration(string schema)
        {
            ToTable(schema + ".VFramework");
            HasKey(x => new { x.FrameworkIdentifier });

            Property(x => x.FrameworkAsset).IsOptional();
            Property(x => x.FrameworkCode).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.FrameworkIdentifier).IsRequired();
            Property(x => x.FrameworkLabel).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.FrameworkSize).IsOptional();
            Property(x => x.FrameworkTitle).IsOptional().IsUnicode(true);
            Property(x => x.OccupationAsset).IsOptional();
            Property(x => x.OccupationCode).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.OccupationIdentifier).IsOptional();
            Property(x => x.OccupationLabel).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.OccupationSize).IsOptional();
            Property(x => x.OccupationTitle).IsOptional().IsUnicode(true);
        }
    }

}
