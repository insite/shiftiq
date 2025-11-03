using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Progress;

public class GradebookConfiguration : IEntityTypeConfiguration<GradebookEntity>
{
    public void Configure(EntityTypeBuilder<GradebookEntity> builder)
    {
        builder.ToTable("QGradebook", "records");
        builder.HasKey(x => new { x.GradebookIdentifier });

        builder.Property(x => x.GradebookIdentifier).HasColumnName("GradebookIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.EventIdentifier).HasColumnName("EventIdentifier");
        builder.Property(x => x.AchievementIdentifier).HasColumnName("AchievementIdentifier");
        builder.Property(x => x.FrameworkIdentifier).HasColumnName("FrameworkIdentifier");
        builder.Property(x => x.GradebookCreated).HasColumnName("GradebookCreated").IsRequired();
        builder.Property(x => x.GradebookTitle).HasColumnName("GradebookTitle").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GradebookType).HasColumnName("GradebookType").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.IsLocked).HasColumnName("IsLocked").IsRequired();
        builder.Property(x => x.Reference).HasColumnName("Reference").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.PeriodIdentifier).HasColumnName("PeriodIdentifier");
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime").IsRequired();
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsRequired().IsUnicode(false).HasMaxLength(100);

    }
}