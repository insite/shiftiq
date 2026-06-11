using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VOrganizationGroupAddressConfiguration : EntityTypeConfiguration<VOrganizationGroupAddress>
    {
        public VOrganizationGroupAddressConfiguration() : this("locations") { }

        public VOrganizationGroupAddressConfiguration(string schema)
        {
            ToTable(schema + ".VOrganizationGroupAddress");
            HasKey(x => new { x.OrganizationIdentifier, x.Country, x.Province, x.City });

            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.Country).IsRequired();
            Property(x => x.Province).IsRequired();
            Property(x => x.City).IsRequired();
            Property(x => x.Occurrences).IsRequired();
        }
    }
}
