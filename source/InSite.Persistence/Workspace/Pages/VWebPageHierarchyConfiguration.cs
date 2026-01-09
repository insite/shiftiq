using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VWebPageHierarchyConfiguration : EntityTypeConfiguration<VWebPageHierarchy>
    {
        public VWebPageHierarchyConfiguration() : this("sites") { }

        public VWebPageHierarchyConfiguration(string schema)
        {
            ToTable(schema + ".VWebPageHierarchy");
            HasKey(x => new { x.WebPageIdentifier });

            Property(x => x.PathIndent).IsRequired().IsUnicode(false).HasMaxLength(6);
            Property(x => x.PathIdentifier).IsRequired().IsUnicode(false);
            Property(x => x.PathUrl).IsRequired().IsUnicode(false);
            Property(x => x.PathSequence).IsRequired().IsUnicode(false);
            Property(x => x.PageSlug).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.WebPageTitle).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.WebPageType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.WebSiteDomain).IsRequired().IsUnicode(false).HasMaxLength(256);
        }
    }
}
