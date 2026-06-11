using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Timeline;

public class ChangeConfiguration : IEntityTypeConfiguration<ChangeEntity>
{
    public void Configure(EntityTypeBuilder<ChangeEntity> builder)
    {
        builder.ToTable("Change", "logs");
        builder.HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });

        builder.Property(x => x.AggregateIdentifier).IsRequired();
        builder.Property(x => x.AggregateVersion).IsRequired();
        builder.Property(x => x.OriginOrganization).IsRequired();
        builder.Property(x => x.OriginUser).IsRequired();
        builder.Property(x => x.ChangeTime).IsRequired();
        builder.Property(x => x.ChangeType).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ChangeData).IsUnicode(true);
    }
}
