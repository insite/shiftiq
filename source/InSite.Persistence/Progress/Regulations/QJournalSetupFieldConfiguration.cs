using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QJournalSetupFieldConfiguration : EntityTypeConfiguration<QJournalSetupField>
    {
        public QJournalSetupFieldConfiguration() : this("records") { }

        public QJournalSetupFieldConfiguration(string schema)
        {
            ToTable(schema + ".QJournalSetupField");
            HasKey(x => new { x.JournalSetupFieldIdentifier });

            HasRequired(a => a.JournalSetup).WithMany(b => b.Fields).HasForeignKey(c => c.JournalSetupIdentifier).WillCascadeOnDelete(false);
        }
    }
}
