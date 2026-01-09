using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TDepartmentStandardConfiguration : EntityTypeConfiguration<TDepartmentStandard>
    {
        public TDepartmentStandardConfiguration() : this("identities") { }

        public TDepartmentStandardConfiguration(string schema)
        {
            ToTable(schema + ".TDepartmentStandard");
            HasKey(x => new { x.DepartmentIdentifier, x.StandardIdentifier });

            Property(x => x.Assigned).IsOptional();
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.StandardIdentifier).IsRequired();

            HasRequired(a => a.Standard).WithMany(b => b.Departments).HasForeignKey(a => a.StandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Department).WithMany(b => b.Standards).HasForeignKey(a => a.DepartmentIdentifier).WillCascadeOnDelete(false);
        }
    }
}
