using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementDepartmentConfiguration : EntityTypeConfiguration<VCmdsAchievementDepartment>
    {
        public VCmdsAchievementDepartmentConfiguration() : this("custom_cmds") { }

        public VCmdsAchievementDepartmentConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsAchievementDepartment");
            HasKey(x => new { x.AchievementIdentifier, x.DepartmentIdentifier });

            HasRequired(x => x.Achievement).WithMany(x => x.Departments).HasForeignKey(x => x.AchievementIdentifier);
        }
    }
}
