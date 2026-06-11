using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TCatalogConfiguration : EntityTypeConfiguration<TCatalog>
    {
        public TCatalogConfiguration()
        {
            ToTable("TCatalog", "learning");
            HasKey(x => new { x.CatalogIdentifier });

            Property(x => x.CatalogIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.CatalogName).IsRequired().IsUnicode(false).HasMaxLength(100);
        }
    }
}
