using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class VJournalSetupUserConfiguration : EntityTypeConfiguration<VJournalSetupUser>
    {
        public VJournalSetupUserConfiguration() : this("records") { }

        public VJournalSetupUserConfiguration(string schema)
        {
            ToTable(schema + ".VJournalSetupUser");
            HasKey(x => new { x.JournalSetupIdentifier, x.UserIdentifier, x.UserRole });
        }
    }
}
