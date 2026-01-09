using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QPersonSecretConfiguration : EntityTypeConfiguration<QPersonSecret>
    {
        public QPersonSecretConfiguration()
        {
            ToTable("QPersonSecret", "contacts");

            HasKey(x => x.SecretIdentifier);

            Property(x => x.PersonIdentifier).IsRequired();
            Property(x => x.SecretIdentifier).IsRequired();
            Property(x => x.SecretType).IsRequired().HasMaxLength(30).IsUnicode(false);
            Property(x => x.SecretName).IsRequired().HasMaxLength(100).IsUnicode(false);
            Property(x => x.SecretExpiry).IsRequired();
            Property(x => x.SecretLifetimeLimit).IsOptional();
            Property(x => x.SecretValue).IsRequired().HasMaxLength(100).IsUnicode(false);

            HasRequired(x => x.Person).WithMany(x => x.Secrets).HasForeignKey(x => x.PersonIdentifier).WillCascadeOnDelete(false);
        }
    }
}
