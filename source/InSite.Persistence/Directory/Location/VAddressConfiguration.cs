using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class VAddressConfiguration : EntityTypeConfiguration<VAddress>
    {
        public VAddressConfiguration() : this("locations") { }

        public VAddressConfiguration(string schema)
        {
            ToTable(schema + ".VAddress");
            HasKey(x => new { x.AddressIdentifier });

            Property(x => x.AddressIdentifier).IsRequired();
            Property(x => x.City).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.Country).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.Description).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.PostalCode).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.Province).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.Street1).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.Street2).IsOptional().IsUnicode(false).HasMaxLength(200);
        }
    }
}
