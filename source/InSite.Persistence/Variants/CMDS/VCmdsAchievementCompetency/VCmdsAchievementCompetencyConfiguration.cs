using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementCompetencyConfiguration : EntityTypeConfiguration<VCmdsAchievementCompetency>
    {
        public VCmdsAchievementCompetencyConfiguration() : this("custom_cmds") { }

        public VCmdsAchievementCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsAchievementCompetency");
            HasKey(x => new { x.AchievementIdentifier, x.CompetencyStandardIdentifier });

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.CompetencyStandardIdentifier).IsRequired();

            HasRequired(x => x.Achievement).WithMany(x => x.Competencies).HasForeignKey(x => x.AchievementIdentifier);
        }
    }
}
