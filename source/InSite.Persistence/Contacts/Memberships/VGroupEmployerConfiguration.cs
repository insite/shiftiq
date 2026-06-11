using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VGroupEmployerConfiguration : EntityTypeConfiguration<VGroupEmployer>
    {
        public VGroupEmployerConfiguration() : this("contacts") { }

        public VGroupEmployerConfiguration(string schema)
        {
            ToTable(schema + ".VGroupEmployer");
            HasKey(x => new { x.GroupIdentifier,x.UserIdentifier });

            Property(x => x.GroupCode).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupIndustry).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupIndustryComment).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.GroupPhone).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupSize).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.AddressCity).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.AddressCountry).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.AddressLine).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.AddressPostalCode).IsOptional().IsUnicode(false).HasMaxLength(16);
            Property(x => x.AddressProvince).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.Email).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.FirstName).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.LastName).IsOptional().IsUnicode(false).HasMaxLength(80);
            Property(x => x.MiddleName).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.Phone1).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.Phone2).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.Url).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.ContactFullName).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.ContactJobTitle).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.Approved).IsOptional();
            Property(x => x.CompanySector).IsOptional().IsUnicode(false);

            Property(x => x.EmployeeHomeAddressIdentifier).IsOptional();
            Property(x => x.EmployeeWorkAddressIdentifier).IsOptional();
            Property(x => x.EmployeeBillingAddressIdentifier).IsOptional();
            Property(x => x.EmployeeShippingAddressIdentifier).IsOptional();

        }
    }
}
