using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TAchievementCategoryConfiguration : EntityTypeConfiguration<TAchievementCategory>
    {
        public TAchievementCategoryConfiguration()
        {
            ToTable("TAchievementCategory", "record");
            HasKey(x => new { x.AchievementIdentifier, x.ItemIdentifier });

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.ItemIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier);
            Property(x => x.CategorySequence);
            Property(x => x.CategoryDescription).IsOptional().IsUnicode(false).HasMaxLength(800);

            HasRequired(a => a.Category).WithMany(b => b.Achievements).HasForeignKey(c => c.ItemIdentifier).WillCascadeOnDelete(false);
        }
    }
}
