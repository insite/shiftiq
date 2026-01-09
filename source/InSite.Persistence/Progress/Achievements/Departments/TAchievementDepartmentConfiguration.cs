using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TAchievementDepartmentConfiguration : EntityTypeConfiguration<TAchievementDepartment>
    {
        public TAchievementDepartmentConfiguration() : this("achievements") { }

        public TAchievementDepartmentConfiguration(string schema)
        {
            ToTable(schema + ".TAchievementDepartment");
            HasKey(x => new { x.JoinIdentifier });
        }
    }
}
