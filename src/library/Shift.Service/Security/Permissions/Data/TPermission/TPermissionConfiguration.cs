using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class TPermissionConfiguration : IEntityTypeConfiguration<TPermissionEntity>
{
    public void Configure(EntityTypeBuilder<TPermissionEntity> builder) 
    {
        builder.ToTable("TGroupPermission", "contacts");
        builder.HasKey(x => new { x.PermissionIdentifier });
            
            builder.Property(x => x.AllowExecute).HasColumnName("AllowExecute").IsRequired();
            builder.Property(x => x.AllowRead).HasColumnName("AllowRead").IsRequired();
            builder.Property(x => x.AllowWrite).HasColumnName("AllowWrite").IsRequired();
            builder.Property(x => x.AllowCreate).HasColumnName("AllowCreate").IsRequired();
            builder.Property(x => x.AllowDelete).HasColumnName("AllowDelete").IsRequired();
            builder.Property(x => x.AllowAdministrate).HasColumnName("AllowAdministrate").IsRequired();
            builder.Property(x => x.AllowConfigure).HasColumnName("AllowConfigure").IsRequired();
            builder.Property(x => x.PermissionMask).HasColumnName("PermissionMask").IsRequired();
            builder.Property(x => x.PermissionGranted).HasColumnName("PermissionGranted");
            builder.Property(x => x.PermissionGrantedBy).HasColumnName("PermissionGrantedBy");
            builder.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier").IsRequired();
            builder.Property(x => x.ObjectType).HasColumnName("ObjectType").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.GroupIdentifier).HasColumnName("GroupIdentifier").IsRequired();
            builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
            builder.Property(x => x.PermissionIdentifier).HasColumnName("PermissionIdentifier").IsRequired();
            builder.Property(x => x.AllowTrialAccess).HasColumnName("AllowTrialAccess").IsRequired();

    }
}