using System.Data.Entity.ModelConfiguration;

using InSite.Application.Sites.Read;

namespace InSite.Persistence
{
    public class QSiteConfigruation : EntityTypeConfiguration<QSite>
    {
        public QSiteConfigruation() : this("sites") { }

        public QSiteConfigruation(string schema)
        {
            ToTable(schema + ".QSite");
            HasKey(x => new { x.SiteIdentifier });

            Property(x => x.LastChangeTime).IsOptional();
            Property(x => x.LastChangeType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.SiteDomain).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.SiteIdentifier).IsRequired();
            Property(x => x.SiteTitle).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(a => a.Organization).WithMany(b => b.Sites).HasForeignKey(c => c.OrganizationIdentifier).WillCascadeOnDelete(false);
        }
    }
}