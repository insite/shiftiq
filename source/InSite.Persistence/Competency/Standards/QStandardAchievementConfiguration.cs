using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardAchievementConfiguration : EntityTypeConfiguration<QStandardAchievement>
    {
        public QStandardAchievementConfiguration() : this("standard") { }

        public QStandardAchievementConfiguration(string schema)
        {
            ToTable("QStandardAchievement", schema);
            HasKey(x => new { x.StandardIdentifier, x.AchievementIdentifier });
        }
    }
}
