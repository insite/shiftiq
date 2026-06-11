using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class OrganizationPermissionConfiguration : EntityTypeConfiguration<OrganizationPermission>
    {
        public OrganizationPermissionConfiguration()
        {
            ToTable("TOrganizationPermission", "security");

            HasKey(e => e.OrganizationId);

            Property(e => e.OrganizationId)
                .IsRequired();

            Property(e => e.AccessGranted)
                .IsOptional()
                .IsUnicode(false);

            Property(e => e.AccessDenied)
                .IsOptional()
                .IsUnicode(false);

            Property(e => e.AccessGrantedToActions)
                .IsOptional()
                .IsUnicode(false);
        }
    }
}
