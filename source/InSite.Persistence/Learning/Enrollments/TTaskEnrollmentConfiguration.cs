using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TTaskEnrollmentConfiguration : EntityTypeConfiguration<TTaskEnrollment>
    {
        public TTaskEnrollmentConfiguration() : this("records") { }

        public TTaskEnrollmentConfiguration(string schema)
        {
            ToTable(schema + ".TTaskEnrollment");
            HasKey(x => new { x.EnrollmentIdentifier });

            Property(x => x.EnrollmentIdentifier).IsRequired();
            Property(x => x.LearnerUserIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.TaskIdentifier).IsRequired();
            Property(x => x.ObjectIdentifier).IsRequired();
            Property(x => x.ProgressCompleted).IsOptional();
            Property(x => x.ProgressStarted).IsOptional();

            HasRequired(x => x.Task).WithMany(x => x.Enrollments).HasForeignKey(x => x.TaskIdentifier);
        }
    }
}
