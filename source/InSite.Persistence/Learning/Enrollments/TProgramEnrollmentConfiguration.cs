using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramEnrollmentConfiguration : EntityTypeConfiguration<TProgramEnrollment>
    {
        public TProgramEnrollmentConfiguration() : this("records") { }

        public TProgramEnrollmentConfiguration(string schema)
        {
            ToTable(schema + ".TProgramEnrollment");
            HasKey(x => new { x.EnrollmentIdentifier });

            Property(x => x.EnrollmentIdentifier).IsRequired();
            Property(x => x.LearnerUserIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.ProgressCompleted).IsOptional();
            Property(x => x.ProgressStarted).IsOptional();

            HasRequired(x => x.Program).WithMany(x => x.Enrollments).HasForeignKey(x => x.ProgramIdentifier);
            HasRequired(x => x.LearnerUser).WithMany(x => x.ProgramEnrollments).HasForeignKey(x => x.LearnerUserIdentifier);
        }
    }
}
