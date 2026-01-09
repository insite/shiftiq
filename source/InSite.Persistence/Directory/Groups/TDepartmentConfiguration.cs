using System.Data.Entity.ModelConfiguration;

using InSite.Application.Identities.Departments.Read;

namespace InSite.Persistence
{
    public class TDepartmentConfiguration : EntityTypeConfiguration<TDepartment>
    {
        public TDepartmentConfiguration() : this("identities") { }

        public TDepartmentConfiguration(string schema)
        {
            ToTable(schema + ".TDepartment");
            HasKey(x => new { x.DepartmentIdentifier });

            Property(x => x.AccessGrantedToCmds).IsRequired();
            Property(x => x.AccessGrantedToPortal).IsRequired();
            Property(x => x.BillingAddressIdentifier).IsOptional();
            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.DepartmentCode).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.DepartmentDescription).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.DepartmentName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DivisionIdentifier).IsOptional();
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.ParentDepartmentIdentifier).IsOptional();
            Property(x => x.PhysicalAddressIdentifier).IsOptional();
            Property(x => x.ShippingAddressIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}