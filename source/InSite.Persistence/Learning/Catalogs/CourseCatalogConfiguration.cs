using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class VCatalogCourseConfiguration : EntityTypeConfiguration<VCatalogCourse>
    {
        public VCatalogCourseConfiguration()
        {
            ToTable("VCatalogCourse", "courses");
            HasKey(x => new { x.CatalogIdentifier, x.CourseIdentifier });

            Property(x => x.CatalogIdentifier).IsRequired();
            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.CatalogName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseCategory).IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseFlagColor).IsUnicode(false).HasMaxLength(10);
            Property(x => x.CourseFlagText).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseImage).IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseLabel).IsUnicode(false).HasMaxLength(20);
            Property(x => x.CourseName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseSlug).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseType).IsUnicode(false).HasMaxLength(20);
            Property(x => x.OrganizationCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.OrganizationName).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseIsHidden).IsRequired();
            Property(x => x.CourseCreated).IsRequired();
            Property(x => x.CourseModified).IsRequired();
        }
    }
}
