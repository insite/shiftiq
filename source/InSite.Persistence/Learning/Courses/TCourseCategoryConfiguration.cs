using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCourseCategoryConfiguration : EntityTypeConfiguration<TCourseCategory>
    {
        public TCourseCategoryConfiguration()
        {
            ToTable("TCourseCategory", "learning");
            HasKey(x => new { x.CourseIdentifier, x.ItemIdentifier });

            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.ItemIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier);
            Property(x => x.CategorySequence);

            HasRequired(a => a.Category).WithMany(b => b.Courses).HasForeignKey(c => c.ItemIdentifier).WillCascadeOnDelete(false);
        }
    }
}
