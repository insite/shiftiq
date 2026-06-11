using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VAchievementCategoryConfiguration : EntityTypeConfiguration<VAchievementCategory>
    {
        public VAchievementCategoryConfiguration() : this("achievements") { }

        public VAchievementCategoryConfiguration(string schema)
        {
            ToTable(schema + ".TAchievementCategory");
            HasKey(x => new { x.CategoryIdentifier });


        }
    }
}
