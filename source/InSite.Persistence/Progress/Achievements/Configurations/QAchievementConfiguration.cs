using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QAchievementConfiguration : EntityTypeConfiguration<QAchievement>
    {
        public QAchievementConfiguration() : this("achievements") { }

        public QAchievementConfiguration(string schema)
        {
            ToTable(schema + ".QAchievement");
            HasKey(x => new { x.AchievementIdentifier });

            Property(x => x.AchievementDescription).IsOptional().IsUnicode(false).HasMaxLength(1200);
            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.AchievementIsEnabled).IsRequired();
            Property(x => x.AchievementLabel).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AchievementTitle).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ExpirationFixedDate).IsOptional();
            Property(x => x.ExpirationLifetimeQuantity).IsOptional();
            Property(x => x.ExpirationLifetimeUnit).IsOptional().IsUnicode(false).HasMaxLength(6);
            Property(x => x.ExpirationType).IsOptional().IsUnicode(false).HasMaxLength(8);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.HasBadgeImage).IsOptional();
            Property(x => x.BadgeImageUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
        }
    }
}