using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementOrganizationConfiguration : EntityTypeConfiguration<VCmdsAchievementOrganization>
    {
        public VCmdsAchievementOrganizationConfiguration() : this("custom_cmds") { }

        public VCmdsAchievementOrganizationConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsAchievementOrganization");
            HasKey(x => new { x.AchievementIdentifier, x.OrganizationIdentifier });

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(x => x.Achievement).WithMany(x => x.Organizations).HasForeignKey(x => x.AchievementIdentifier);
        }
    }
}
