using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class TUserFieldConfiguration : IEntityTypeConfiguration<TUserFieldEntity>
{
    public void Configure(EntityTypeBuilder<TUserFieldEntity> builder)
    {
        builder.ToTable("TUserSetting", "accounts");
        builder.HasKey(x => new { x.SettingIdentifier });

        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.Name).HasColumnName("Name").IsRequired().IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.ValueType).HasColumnName("ValueType").IsRequired().IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.ValueJson).HasColumnName("ValueJson").IsRequired().IsUnicode(false);
        builder.Property(x => x.SettingIdentifier).HasColumnName("SettingIdentifier").IsRequired();

    }
}