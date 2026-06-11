using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TSenderConfiguration : EntityTypeConfiguration<TSender>
    {
        public TSenderConfiguration() : this("accounts") { }

        public TSenderConfiguration(string schema)
        {
            ToTable(schema + ".TSender");

            HasKey(x => new { x.SenderIdentifier });

            Property(x => x.SenderIdentifier)
                .IsRequired();

            Property(x => x.SenderType)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.SenderNickname)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.SenderName)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.SenderEmail)
                .HasMaxLength(254)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.SystemMailbox)
                .HasMaxLength(254)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.CompanyAddress)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.CompanyCity)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.CompanyPostalCode)
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.CompanyCountry)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);
        }
    }
}
