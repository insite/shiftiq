using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Metadata;

public class TActionConfiguration : IEntityTypeConfiguration<TActionEntity>
{
    public void Configure(EntityTypeBuilder<TActionEntity> builder)
    {
        builder.ToTable("TAction", "settings");
        builder.HasKey(x => new { x.ActionIdentifier });

        builder.Property(x => x.ActionIdentifier).HasColumnName("ActionIdentifier").IsRequired();
        builder.Property(x => x.NavigationParentActionIdentifier).HasColumnName("NavigationParentActionIdentifier");
        builder.Property(x => x.PermissionParentActionIdentifier).HasColumnName("PermissionParentActionIdentifier");

        builder.Property(x => x.ActionIcon).HasColumnName("ActionIcon").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.ActionList).HasColumnName("ActionList").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.ActionName).HasColumnName("ActionName").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ActionNameShort).HasColumnName("ActionNameShort").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.ActionType).HasColumnName("ActionType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.ActionUrl).HasColumnName("ActionUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.AuthorizationRequirement).HasColumnName("AuthorizationRequirement").IsUnicode(false).HasMaxLength(34);
        builder.Property(x => x.AuthorityType).HasColumnName("AuthorityType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.ControllerPath).HasColumnName("ControllerPath").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ExtraBreadcrumb).HasColumnName("ExtraBreadcrumb").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.HelpUrl).HasColumnName("HelpUrl").IsUnicode(false).HasMaxLength(500);
    }
}