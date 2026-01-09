using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Progress;

public class QGradebookEnrollmentConfiguration : IEntityTypeConfiguration<QGradebookEnrollmentEntity>
{
    public void Configure(EntityTypeBuilder<QGradebookEnrollmentEntity> builder)
    {
        builder.ToTable("QEnrollment", "records");
        builder.HasKey(x => new { x.EnrollmentIdentifier });

        builder.Property(x => x.GradebookIdentifier).HasColumnName("GradebookIdentifier").IsRequired();
        builder.Property(x => x.LearnerIdentifier).HasColumnName("LearnerIdentifier").IsRequired();
        builder.Property(x => x.PeriodIdentifier).HasColumnName("PeriodIdentifier");
        builder.Property(x => x.EnrollmentIdentifier).HasColumnName("EnrollmentIdentifier").IsRequired();
        builder.Property(x => x.EnrollmentStarted).HasColumnName("EnrollmentStarted");
        builder.Property(x => x.EnrollmentComment).HasColumnName("EnrollmentComment").IsUnicode(false).HasMaxLength(400);
        builder.Property(x => x.EnrollmentRestart).HasColumnName("EnrollmentRestart").IsRequired();
        builder.Property(x => x.EnrollmentCompleted).HasColumnName("EnrollmentCompleted");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}