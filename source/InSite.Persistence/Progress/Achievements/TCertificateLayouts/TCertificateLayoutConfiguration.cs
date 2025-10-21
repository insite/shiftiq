using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCertificateLayoutConfiguration : EntityTypeConfiguration<TCertificateLayout>
    {
        public TCertificateLayoutConfiguration() : this("records") { }

        public TCertificateLayoutConfiguration(string schema)
        {
            ToTable(schema + ".TCertificateLayout");
            HasKey(x => new { x.CertificateLayoutIdentifier });

            Property(x => x.CertificateLayoutCode).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CertificateLayoutData).IsRequired().IsUnicode(true).HasMaxLength(1200);
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}