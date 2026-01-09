using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class VCourseConfiguration : EntityTypeConfiguration<VCourse>
    {
        public VCourseConfiguration() : this("courses") { }

        public VCourseConfiguration(string schema)
        {
            ToTable(schema + ".VCourse");
            HasKey(x => new { x.CourseIdentifier });

            Property(x => x.CourseCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.CourseHook).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseLabel).IsUnicode(false).HasMaxLength(20);
            Property(x => x.CourseName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.GradebookTitle).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CatalogName).IsUnicode(false).HasMaxLength(100);
        }
    }
}
