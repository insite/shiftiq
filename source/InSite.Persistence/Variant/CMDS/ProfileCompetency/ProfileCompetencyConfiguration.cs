using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ProfileCompetencyConfiguration : EntityTypeConfiguration<ProfileCompetency>
    {
        public ProfileCompetencyConfiguration() : this("custom_cmds") { }

        public ProfileCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".ProfileCompetency");
            HasKey(x => new { x.CompetencyStandardIdentifier, x.ProfileStandardIdentifier });
        
            Property(x => x.CertificationHoursCore).IsOptional();
            Property(x => x.CertificationHoursNonCore).IsOptional();
            Property(x => x.CompetencyStandardIdentifier).IsRequired();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
        }
    }
}
