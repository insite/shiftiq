using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QJournalSetupGroupConfiguration : EntityTypeConfiguration<QJournalSetupGroup>
    {
        public QJournalSetupGroupConfiguration() : this("records") { }

        public QJournalSetupGroupConfiguration(string schema)
        {
            ToTable(schema + ".QJournalSetupGroup");
            HasKey(x => new { x.JournalSetupIdentifier, x.GroupIdentifier });

            HasRequired(a => a.JournalSetup).WithMany(b => b.Groups).HasForeignKey(c => c.JournalSetupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Group).WithMany(b => b.JournalSetupGroups).HasForeignKey(c => c.GroupIdentifier).WillCascadeOnDelete(false);
        }
    }
}
