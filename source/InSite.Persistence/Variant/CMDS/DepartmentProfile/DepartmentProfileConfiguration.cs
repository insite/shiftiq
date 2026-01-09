using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class DepartmentProfileConfiguration : EntityTypeConfiguration<DepartmentProfile>
    {
        public DepartmentProfileConfiguration() : this("custom_cmds") { }

        public DepartmentProfileConfiguration(string schema)
        {
            ToTable(schema + ".DepartmentProfile");
            HasKey(x => new { x.DepartmentIdentifier, x.ProfileStandardIdentifier });
        
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
        }
    }
}
