using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QAchievementPrerequisiteConfiguration : EntityTypeConfiguration<QAchievementPrerequisite>
    {
        public QAchievementPrerequisiteConfiguration() : this("records") { }

        public QAchievementPrerequisiteConfiguration(string schema)
        {
            ToTable(schema + ".QAchievementPrerequisite");
            HasKey(x => x.PrerequisiteIdentifier);

            HasRequired(a => a.Achievement).WithMany(b => b.Prerequisites).HasForeignKey(c => c.AchievementIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.PrerequisiteAchievementIdentifier).IsRequired();
            Property(x => x.PrerequisiteIdentifier).IsRequired();
        }
    }
}
