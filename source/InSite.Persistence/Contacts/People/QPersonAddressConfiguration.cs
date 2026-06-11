using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QPersonAddressConfiguration : EntityTypeConfiguration<QPersonAddress>
    {
        public QPersonAddressConfiguration()
        {
            ToTable("QPersonAddress", "contacts");
            HasKey(x => new { x.AddressIdentifier });

            Property(x => x.AddressIdentifier).IsRequired();
            Property(x => x.City).IsUnicode(false).HasMaxLength(128);
            Property(x => x.Country).IsUnicode(false).HasMaxLength(32);
            Property(x => x.Description).IsUnicode(false).HasMaxLength(128);
            Property(x => x.PostalCode).IsUnicode(false).HasMaxLength(20);
            Property(x => x.Province).IsUnicode(false).HasMaxLength(64);
            Property(x => x.Street1).IsUnicode(false).HasMaxLength(200);
            Property(x => x.Street2).IsUnicode(false).HasMaxLength(200);
        }
    }
}
