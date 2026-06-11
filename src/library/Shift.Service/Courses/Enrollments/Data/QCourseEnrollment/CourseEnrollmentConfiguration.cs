using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Learning;

public class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollmentEntity>
{
    public void Configure(EntityTypeBuilder<CourseEnrollmentEntity> builder)
    {
        builder.ToTable("QCourseEnrollment", "courses");
        builder.HasKey(x => new { x.CourseIdentifier });
    }
}