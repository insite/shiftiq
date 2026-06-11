using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsAchievementCategoryConfiguration : EntityTypeConfiguration<VCmdsAchievementCategory>
    {
        public VCmdsAchievementCategoryConfiguration() : this("custom_cmds") { }

        public VCmdsAchievementCategoryConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsAchievementCategory");
            HasKey(x => new { x.AchievementIdentifier, x.CategoryIdentifier });

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.CategoryIdentifier).IsRequired();
            Property(x => x.CategoryName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.ClassificationSequence).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(x => x.Achievement).WithMany(x => x.Categories).HasForeignKey(x => x.AchievementIdentifier);
        }
    }
}
