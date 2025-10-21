using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QCoursePrerequisiteConfiguration : EntityTypeConfiguration<QCoursePrerequisite>
    {
        public QCoursePrerequisiteConfiguration() : this("courses") { }

        public QCoursePrerequisiteConfiguration(string schema)
        {
            ToTable(schema + ".QCoursePrerequisite");
            HasKey(x => new { x.CoursePrerequisiteIdentifier });

            Property(x => x.ObjectType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.TriggerChange).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.TriggerType).IsRequired().IsUnicode(false).HasMaxLength(30);

            HasRequired(a => a.Course).WithMany(b => b.Prerequisites).HasForeignKey(a => a.CourseIdentifier).WillCascadeOnDelete(false);
        }
    }
}
