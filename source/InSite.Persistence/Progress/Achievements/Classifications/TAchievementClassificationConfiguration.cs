using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VAchievementClassificationConfiguration : EntityTypeConfiguration<VAchievementClassification>
    {
        public VAchievementClassificationConfiguration() : this("achievements") { }

        public VAchievementClassificationConfiguration(string schema)
        {
            ToTable(schema + ".TAchievementClassification");
            HasKey(x => new { x.AchievementIdentifier, x.CategoryIdentifier });

            HasRequired(x => x.Category).WithMany(x => x.Classifications).HasForeignKey(x => x.CategoryIdentifier);
        }
    }
}
