using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class EmploymentConfiguration : EntityTypeConfiguration<Employment>
    {
        public EmploymentConfiguration() : this("custom_cmds") { }

        public EmploymentConfiguration(string schema)
        {
            ToTable(schema + ".Employment");
            HasKey(x => new { x.DepartmentIdentifier, x.UserIdentifier });
        
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
