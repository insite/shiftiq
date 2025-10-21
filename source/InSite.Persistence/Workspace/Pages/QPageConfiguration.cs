using System.Data.Entity.ModelConfiguration;

using InSite.Application.Sites.Read;

namespace InSite.Persistence
{
    public class QPageConfiguration : EntityTypeConfiguration<QPage>
    {
        public QPageConfiguration() : this("sites") { }

        public QPageConfiguration(string schema)
        {
            ToTable(schema + ".QPage");
            HasKey(x => x.PageIdentifier);

            Property(x => x.AuthorDate).IsOptional();
            Property(x => x.AuthorName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ContentControl).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ContentLabels).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.Hook).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.IsHidden).IsRequired();
            Property(x => x.IsNewTab).IsRequired();
            Property(x => x.NavigateUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.PageIcon).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.PageIdentifier).IsRequired();
            Property(x => x.PageSlug).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.PageType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.ParentPageIdentifier).IsOptional();
            Property(x => x.ObjectIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.Sequence).IsRequired();
            Property(x => x.SiteIdentifier).IsOptional();
            Property(x => x.Title).IsRequired().IsUnicode(true).HasMaxLength(128);
            
            Property(x => x.LastChangeTime).IsOptional();
            Property(x => x.LastChangeType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsOptional().IsUnicode(false).HasMaxLength(100);
        }
    }
}
