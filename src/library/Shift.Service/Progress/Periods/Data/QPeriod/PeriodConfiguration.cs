using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Progress;

public class PeriodConfiguration : IEntityTypeConfiguration<PeriodEntity>
{
    public void Configure(EntityTypeBuilder<PeriodEntity> builder)
    {
        builder.ToTable("QPeriod", "records");
        builder.HasKey(x => new { x.PeriodIdentifier });

        builder.Property(x => x.PeriodIdentifier).HasColumnName("PeriodIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.PeriodName).HasColumnName("PeriodName").IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.PeriodStart).HasColumnName("PeriodStart").IsRequired();
        builder.Property(x => x.PeriodEnd).HasColumnName("PeriodEnd").IsRequired();
    }
}