using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class TPartitionFieldConfiguration : IEntityTypeConfiguration<TPartitionFieldEntity>
{
    public void Configure(EntityTypeBuilder<TPartitionFieldEntity> builder)
    {
        builder.ToTable("TPartitionSetting", "security");
        builder.HasKey(x => new { x.SettingIdentifier });

        builder.Property(x => x.SettingIdentifier).HasColumnName("SettingIdentifier").IsRequired();
        builder.Property(x => x.SettingName).HasColumnName("SettingName").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.SettingValue).HasColumnName("SettingValue").IsRequired().IsUnicode(false).HasMaxLength(500);

    }
}