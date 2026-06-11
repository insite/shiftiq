using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VSitemapConfiguration : EntityTypeConfiguration<VSitemap>
    {
        public VSitemapConfiguration() : this("sites") { }

        public VSitemapConfiguration(string schema)
        {
            ToTable(schema + ".VSitemap");
            HasKey(x => new { x.PageIdentifier });

            Property(x => x.PageTitle).HasMaxLength(128);
            Property(x => x.PageTitleIndented).HasMaxLength(4000);
            Property(x => x.PageType).IsUnicode(false).HasMaxLength(64);
            Property(x => x.PathSequence).IsUnicode(false);
            Property(x => x.PathUrl).IsUnicode(false);
            Property(x => x.SiteDomain).IsRequired().IsUnicode(false).HasMaxLength(256);
        }
    }
}
