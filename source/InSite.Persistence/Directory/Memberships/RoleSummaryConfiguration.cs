using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class RoleSummaryConfiguration : EntityTypeConfiguration<RoleSummary>
    {
        public RoleSummaryConfiguration() : this("contacts") { }

        public RoleSummaryConfiguration(string schema)
        {
            ToTable(schema + ".RoleSummary");
            HasKey(x => new { x.GroupIdentifier });
        
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.GroupTenantCode).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupTenantIdentifier).IsRequired();
            Property(x => x.GroupTenantName).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.GroupTenantTitle).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.GroupType).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.RoleAssigned).IsRequired();
            Property(x => x.RoleName).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.UserEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFirstName).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.UserHasPassword).IsRequired();
            Property(x => x.UserIsArchived).IsOptional();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserLastName).IsOptional().IsUnicode(false).HasMaxLength(80);
        }
    }
}
