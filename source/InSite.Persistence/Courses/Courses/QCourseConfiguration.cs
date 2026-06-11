using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QCourseConfiguration : EntityTypeConfiguration<QCourse>
    {
        public QCourseConfiguration() : this("courses") { }

        public QCourseConfiguration(string schema)
        {
            ToTable(schema + ".QCourse");
            HasKey(x => new { x.CourseIdentifier });

            Property(x => x.CourseCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.CourseDescription).IsUnicode(false);
            Property(x => x.CourseHook).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseIcon).IsUnicode(false).HasMaxLength(30);
            Property(x => x.CourseImage).IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseLabel).IsUnicode(false).HasMaxLength(20);
            Property(x => x.CourseLevel).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseProgram).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseSlug).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseStyle).IsUnicode(false);
            Property(x => x.CourseUnit).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseFlagColor).IsUnicode(false).HasMaxLength(10);
            Property(x => x.CourseFlagText).IsUnicode(false).HasMaxLength(50);

            HasOptional(a => a.Catalog).WithMany(b => b.Courses).HasForeignKey(a => a.CatalogIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Gradebook).WithMany(b => b.Courses).HasForeignKey(a => a.GradebookIdentifier).WillCascadeOnDelete(false);
        }
    }
}