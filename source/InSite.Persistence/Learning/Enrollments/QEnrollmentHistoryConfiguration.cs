using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QEnrollmentHistoryConfiguration : EntityTypeConfiguration<QEnrollmentHistory>
    {
        public QEnrollmentHistoryConfiguration() : this("records") { }

        public QEnrollmentHistoryConfiguration(string schema)
        {
            ToTable(schema + ".QEnrollmentHistory");
            HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });

            Property(x => x.AggregateIdentifier).IsRequired();
            Property(x => x.AggregateVersion).IsRequired();
            Property(x => x.ChangeBy).IsRequired();
            Property(x => x.ChangeTime).IsRequired();
            Property(x => x.EnrollmentIdentifier).IsRequired();
            Property(x => x.EnrollmentTime).IsOptional();
            Property(x => x.EnrollmentType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.LearnerIdentifier).IsRequired();
        }
    }
}
