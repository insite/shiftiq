using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Progress;

public class AchievementConfiguration : IEntityTypeConfiguration<AchievementEntity>
{
    public void Configure(EntityTypeBuilder<AchievementEntity> builder)
    {
        builder.ToTable("QAchievement", "achievements");
        builder.HasKey(x => new { x.AchievementIdentifier });

        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.AchievementIdentifier).HasColumnName("AchievementIdentifier").IsRequired();
        builder.Property(x => x.AchievementLabel).HasColumnName("AchievementLabel").IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.AchievementTitle).HasColumnName("AchievementTitle").IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.AchievementDescription).HasColumnName("AchievementDescription").IsUnicode(false).HasMaxLength(1200);
        builder.Property(x => x.AchievementIsEnabled).HasColumnName("AchievementIsEnabled").IsRequired();
        builder.Property(x => x.ExpirationType).HasColumnName("ExpirationType").IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.ExpirationFixedDate).HasColumnName("ExpirationFixedDate");
        builder.Property(x => x.ExpirationLifetimeQuantity).HasColumnName("ExpirationLifetimeQuantity");
        builder.Property(x => x.ExpirationLifetimeUnit).HasColumnName("ExpirationLifetimeUnit").IsUnicode(false).HasMaxLength(6);
        builder.Property(x => x.CertificateLayoutCode).HasColumnName("CertificateLayoutCode").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.AchievementType).HasColumnName("AchievementType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.AchievementReportingDisabled).HasColumnName("AchievementReportingDisabled").IsRequired();
        builder.Property(x => x.BadgeImageUrl).HasColumnName("BadgeImageUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.HasBadgeImage).HasColumnName("HasBadgeImage");
        builder.Property(x => x.AchievementAllowSelfDeclared).HasColumnName("AchievementAllowSelfDeclared").IsRequired();

    }
}