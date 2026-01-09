using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class TCollegeCertificateConfiguration : EntityTypeConfiguration<TCollegeCertificate>
    {
        public TCollegeCertificateConfiguration() : this("custom_cmds") { }

        public TCollegeCertificateConfiguration(string schema)
        {
            ToTable(schema + ".TCollegeCertificate");
            HasKey(x => new { x.CertificateIdentifier });

            Property(x => x.CertificateAuthority).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CertificateIdentifier).IsRequired();
            Property(x => x.CertificateTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DateGranted).IsOptional();
            Property(x => x.DateRequested).IsOptional();
            Property(x => x.DateSubmitted).IsOptional();
            Property(x => x.LearnerIdentifier).IsRequired();
            Property(x => x.ProfileIdentifier).IsRequired();
        }
    }
}
