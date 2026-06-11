using System.Data.Entity.ModelConfiguration;

using InSite.Application.Files.Read;

namespace InSite.Persistence
{
    public class TFileConfiguration : EntityTypeConfiguration<TFile>
    {
        public TFileConfiguration() : this("assets") { }

        public TFileConfiguration(string schema)
        {
            ToTable(schema + ".TFile");
            HasKey(x => new { x.FileIdentifier });

            Property(x => x.ObjectType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.FileName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.FileLocation).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.FileUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.FilePath).IsOptional().IsUnicode(false).HasMaxLength(260);
            Property(x => x.FileContentType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.FileCategory).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.FileSubcategory).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.FileStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
        }
    }
}
