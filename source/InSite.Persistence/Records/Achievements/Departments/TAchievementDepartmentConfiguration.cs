using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TAchievementDepartmentConfiguration : EntityTypeConfiguration<TAchievementDepartment>
    {
        public TAchievementDepartmentConfiguration() : this("achievements") { }

        public TAchievementDepartmentConfiguration(string schema)
        {
            ToTable(schema + ".TAchievementDepartment");
            HasKey(x => new { x.JoinIdentifier });

            HasRequired(x => x.Achievement).WithMany(x => x.Departments).HasForeignKey(x => x.AchievementIdentifier);
            HasRequired(x => x.Department).WithMany(x => x.AchievementDepartments).HasForeignKey(x => x.DepartmentIdentifier);
        }
    }
}
