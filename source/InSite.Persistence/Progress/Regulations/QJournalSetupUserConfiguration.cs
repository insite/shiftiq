using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QJournalSetupUserConfiguration : EntityTypeConfiguration<QJournalSetupUser>
    {
        public QJournalSetupUserConfiguration() : this("records") { }

        public QJournalSetupUserConfiguration(string schema)
        {
            ToTable(schema + ".QJournalSetupUser");
            HasKey(x => x.EnrollmentIdentifier);

            Property(x => x.EnrollmentIdentifier).IsRequired();

            HasRequired(a => a.JournalSetup).WithMany(b => b.Users).HasForeignKey(c => c.JournalSetupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany(b => b.JournalSetupUsers).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
