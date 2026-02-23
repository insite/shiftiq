using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VOrganizationPersonAddressConfiguration : EntityTypeConfiguration<VOrganizationPersonAddress>
    {
        public VOrganizationPersonAddressConfiguration() : this("locations") { }

        public VOrganizationPersonAddressConfiguration(string schema)
        {
            ToTable(schema + ".VOrganizationPersonAddress");
            HasKey(x => new { x.OrganizationIdentifier, x.Country, x.Province, x.City });

            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.Country).IsRequired();
            Property(x => x.Province).IsRequired();
            Property(x => x.City).IsRequired();
            Property(x => x.Occurrences).IsRequired();
        }
    }
}
