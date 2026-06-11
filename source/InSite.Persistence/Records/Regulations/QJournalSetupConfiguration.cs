using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QJournalSetupConfiguration : EntityTypeConfiguration<QJournalSetup>
    {
        public QJournalSetupConfiguration() : this("records") { }

        public QJournalSetupConfiguration(string schema)
        {
            ToTable(schema + ".QJournalSetup");
            HasKey(x => new { x.JournalSetupIdentifier });

            HasOptional(a => a.Achievement).WithMany(b => b.JournalSetups).HasForeignKey(c => c.AchievementIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Event).WithMany(b => b.JournalSetups).HasForeignKey(c => c.EventIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Framework).WithMany(b => b.JournalSetups).HasForeignKey(c => c.FrameworkStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
