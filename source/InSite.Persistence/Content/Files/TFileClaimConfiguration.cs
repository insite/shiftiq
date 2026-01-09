using System.Data.Entity.ModelConfiguration;

using InSite.Application.Files.Read;

namespace InSite.Persistence
{
    public class TFileClaimConfiguration : EntityTypeConfiguration<TFileClaim>
    {
        public TFileClaimConfiguration() : this("assets") { }

        public TFileClaimConfiguration(string schema)
        {
            ToTable(schema + ".TFileClaim");
            HasKey(x => new { x.ClaimIdentifier });

            Property(x => x.ObjectType).IsRequired().IsUnicode(false).HasMaxLength(100);

            HasRequired(a => a.File).WithMany(b => b.FileClaims).HasForeignKey(c => c.FileIdentifier).WillCascadeOnDelete(false);
        }
    }
}
