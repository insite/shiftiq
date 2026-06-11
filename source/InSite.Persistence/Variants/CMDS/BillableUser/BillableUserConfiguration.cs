using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class BillableUserConfiguration : EntityTypeConfiguration<BillableUser>
    {
        public BillableUserConfiguration() : this("custom_cmds") { }

        public BillableUserConfiguration(string schema)
        {
            ToTable(schema + ".BillableUser");
            HasKey(x => new { x.UserIdentifier });
        
            Property(x => x.BillingClassification).IsRequired().IsUnicode(false).HasMaxLength(1);
            Property(x => x.City).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.CompanyName).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.Email).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.FirstName).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.LastName).IsOptional().IsUnicode(false).HasMaxLength(80);
            Property(x => x.Phone).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.PostalCode).IsOptional().IsUnicode(false).HasMaxLength(16);
            Property(x => x.ProfileCount).IsOptional();
            Property(x => x.Province).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.Street).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.TimeSensitiveResourceCount).IsOptional();
            Property(x => x.UserIsApproved).IsOptional();
            Property(x => x.UserIsDisabled).IsOptional();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserName).IsOptional().IsUnicode(false).HasMaxLength(254);
        }
    }
}
