using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TAchievementOrganizationConfiguration : EntityTypeConfiguration<TAchievementOrganization>
    {
        public TAchievementOrganizationConfiguration() : this("achievements") { }

        public TAchievementOrganizationConfiguration(string schema)
        {
            ToTable(schema + ".TAchievementOrganization");
            HasKey(x => new { x.AchievementIdentifier, x.OrganizationIdentifier });
        }
    }
}
