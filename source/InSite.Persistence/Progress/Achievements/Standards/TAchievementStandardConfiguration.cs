using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TAchievementStandardConfiguration : EntityTypeConfiguration<TAchievementStandard>
    {
        public TAchievementStandardConfiguration() : this("achievements") { }

        public TAchievementStandardConfiguration(string schema)
        {
            ToTable(schema + ".TAchievementStandard");
            HasKey(x => new { x.AchievementIdentifier, x.StandardIdentifier });
        }
    }
}
