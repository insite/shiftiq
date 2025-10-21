using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsProfileOrganizationConfiguration : EntityTypeConfiguration<VCmdsProfileOrganization>
    {
        public VCmdsProfileOrganizationConfiguration() : this("custom_cmds") { }

        public VCmdsProfileOrganizationConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsProfileOrganization");
            HasKey(x => new { x.ProfileStandardIdentifier });

            Property(x => x.ProfileStandardIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
