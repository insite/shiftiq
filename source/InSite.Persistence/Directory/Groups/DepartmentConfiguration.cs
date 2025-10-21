using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class DepartmentConfiguration : EntityTypeConfiguration<Department>
    {
        public DepartmentConfiguration() : this("identities") { }

        public DepartmentConfiguration(string schema)
        {
            ToTable(schema + ".Department");
            HasKey(x => new { x.DepartmentIdentifier });

            Property(x => x.GroupCreated).IsRequired();
            Property(x => x.DepartmentCode).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.DepartmentDescription).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.DepartmentName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DivisionIdentifier).IsOptional();
            Property(x => x.LastChangeTime).IsRequired();
            Property(x => x.LastChangeUser).IsRequired();
            Property(x => x.ParentDepartmentIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasOptional(a => a.Division).WithMany(b => b.Departments).HasForeignKey(c => c.DivisionIdentifier).WillCascadeOnDelete(false);
        }
    }
}
