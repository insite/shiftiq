using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class CompanyDepartmentConfiguration : EntityTypeConfiguration<CompanyDepartment>
    {
        public CompanyDepartmentConfiguration() : this("contacts") { }

        public CompanyDepartmentConfiguration(string schema)
        {
            ToTable(schema + ".CompanyDepartment");
            HasKey(x => new { x.CompanyKey, x.DepartmentIdentifier });
        
            Property(x => x.CompanyKey).IsRequired();
            Property(x => x.CompanyName).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.DepartmentName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}