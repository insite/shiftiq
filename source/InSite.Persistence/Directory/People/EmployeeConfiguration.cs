using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration() : this("reports") { }

        public EmployeeConfiguration(string schema)
        {
            ToTable(schema + ".Employee");
            HasKey(x => new { x.EmployeeUserIdentifier, x.EmployeeOrganizationIdentifier });
        }
    }
}