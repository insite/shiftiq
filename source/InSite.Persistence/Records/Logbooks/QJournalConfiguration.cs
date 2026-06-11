using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QJournalConfiguration : EntityTypeConfiguration<QJournal>
    {
        public QJournalConfiguration() : this("records") { }

        public QJournalConfiguration(string schema)
        {
            ToTable(schema + ".QJournal");
            HasKey(x => new { x.JournalIdentifier });

            HasRequired(a => a.JournalSetup).WithMany(b => b.Journals).HasForeignKey(c => c.JournalSetupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany(b => b.Journals).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
