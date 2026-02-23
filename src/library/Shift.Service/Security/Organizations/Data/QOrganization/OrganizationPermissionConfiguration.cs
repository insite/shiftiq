using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class OrganizationPermissionConfiguration : IEntityTypeConfiguration<OrganizationPermissionEntity>
{
    public void Configure(EntityTypeBuilder<OrganizationPermissionEntity> builder)
    {
        builder.ToTable("TOrganizationPermission", "security");
        builder.HasKey(x => x.OrganizationId);
    }
}