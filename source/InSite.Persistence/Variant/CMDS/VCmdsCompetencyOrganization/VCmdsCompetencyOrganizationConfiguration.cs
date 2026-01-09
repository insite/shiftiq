using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsCompetencyOrganizationConfiguration : EntityTypeConfiguration<VCmdsCompetencyOrganization>
    {
        public VCmdsCompetencyOrganizationConfiguration() : this("custom_cmds") { }

        public VCmdsCompetencyOrganizationConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsCompetencyOrganization");
            HasKey(x => new { x.CompetencyStandardIdentifier, x.OrganizationIdentifier });

            Property(x => x.CompetencyStandardIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
