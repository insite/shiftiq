using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QGroupEnrollmentConfiguration : EntityTypeConfiguration<QGroupEnrollment>
    {
        public QGroupEnrollmentConfiguration() : this("records") { }

        public QGroupEnrollmentConfiguration(string schema)
        {
            ToTable("QGroupEnrollment", schema);
            HasKey(x => x.GroupEnrollmentIdentifier);

            HasRequired(a => a.Gradebook).WithMany(b => b.GroupEnrollments).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Group).WithMany(b => b.GroupEnrollments).HasForeignKey(c => c.GroupIdentifier).WillCascadeOnDelete(false);
        }
    }
}
