using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class EmployerConfiguration : EntityTypeConfiguration<Employer>
    {
        public EmployerConfiguration() : this("reports") { }

        public EmployerConfiguration(string schema)
        {
            ToTable(schema + ".Employer");
            HasKey(x => new { x.EmployerGroupIdentifier });

            Property(x => x.EmployeeCount).IsOptional();
            Property(x => x.EmployerDistrictName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.EmployerGroupCategory).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.EmployerGroupIdentifier).IsRequired();
            Property(x => x.EmployerGroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.EmployerOrganizationIdentifier).IsRequired();
            Property(x => x.EmployerOrganizationName).IsRequired().IsUnicode(false).HasMaxLength(64);
        }
    }
}