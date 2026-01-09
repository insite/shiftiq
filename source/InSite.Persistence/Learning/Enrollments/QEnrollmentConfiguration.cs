using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QEnrollmentConfiguration : EntityTypeConfiguration<QEnrollment>
    {
        public QEnrollmentConfiguration() : this("records") { }

        public QEnrollmentConfiguration(string schema)
        {
            ToTable(schema + ".QEnrollment");
            HasKey(x => x.EnrollmentIdentifier);

            Property(x => x.EnrollmentComment).IsOptional().IsUnicode(false).HasMaxLength(400);
            Property(x => x.EnrollmentIdentifier).IsRequired();
            Property(x => x.EnrollmentRestart).IsRequired();
            Property(x => x.EnrollmentStarted).IsOptional();
            Property(x => x.EnrollmentCompleted).IsOptional();
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.LearnerIdentifier).IsRequired();
            Property(x => x.PeriodIdentifier).IsOptional();

            HasRequired(a => a.Gradebook).WithMany(b => b.Enrollments).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Learner).WithMany(b => b.Enrollments).HasForeignKey(c => c.LearnerIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Period).WithMany(b => b.Enrollments).HasForeignKey(c => c.PeriodIdentifier).WillCascadeOnDelete(false);
        }
    }
}
