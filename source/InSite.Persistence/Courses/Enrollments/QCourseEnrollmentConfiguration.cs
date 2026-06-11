using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QCourseEnrollmentConfiguration : EntityTypeConfiguration<QCourseEnrollment>
    {
        public QCourseEnrollmentConfiguration() : this("courses") { }

        public QCourseEnrollmentConfiguration(string schema)
        {
            ToTable(schema + ".QCourseEnrollment");
            HasKey(x => x.CourseEnrollmentIdentifier);

            HasRequired(a => a.Course).WithMany(b => b.Enrollments).HasForeignKey(a => a.CourseIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.LearnerUser).WithMany(b => b.CourseEnrollments).HasForeignKey(a => a.LearnerUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}