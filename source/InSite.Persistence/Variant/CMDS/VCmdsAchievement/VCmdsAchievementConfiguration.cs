using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementConfiguration : EntityTypeConfiguration<VCmdsAchievement>
    {
        public VCmdsAchievementConfiguration() : this("custom_cmds") { }

        public VCmdsAchievementConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsAchievement");
            HasKey(x => new { x.AchievementIdentifier });

            Property(x => x.AchievementDescription).IsOptional().IsUnicode(false);
            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.AchievementLabel).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AchievementTitle).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ValidForCount).IsOptional();
            Property(x => x.ValidForUnit).IsOptional().IsUnicode(false).HasMaxLength(5);
            Property(x => x.Visibility).IsRequired().IsUnicode(false).HasMaxLength(22);
        }
    }
}
