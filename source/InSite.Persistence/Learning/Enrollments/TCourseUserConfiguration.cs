using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TCourseUserConfiguration : EntityTypeConfiguration<TCourseUser>
    {
        public TCourseUserConfiguration() : this("courses") { }

        public TCourseUserConfiguration(string schema)
        {
            ToTable(schema + ".TCourseUser");
            HasKey(x => x.EnrollmentIdentifier);

            Property(x => x.EnrollmentIdentifier).IsRequired();

            HasRequired(a => a.Course).WithMany(b => b.Users).HasForeignKey(a => a.CourseIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany(b => b.CourseUsers).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}