using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class QGroupAddressConfiguration : EntityTypeConfiguration<QGroupAddress>
    {
        public QGroupAddressConfiguration() : this("contacts") { }

        public QGroupAddressConfiguration(string schema)
        {
            ToTable(schema + ".QGroupAddress");
            HasKey(x => x.AddressIdentifier);

            Property(x => x.AddressIdentifier).IsRequired();

            HasRequired(a => a.Group).WithMany(b => b.Addresses).HasForeignKey(c => c.GroupIdentifier).WillCascadeOnDelete(false);
        }
    }
}