using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QCredentialHistoryConfiguration : EntityTypeConfiguration<QCredentialHistory>
    {
        public QCredentialHistoryConfiguration() : this("records") { }

        public QCredentialHistoryConfiguration(string schema)
        {
            ToTable(schema + ".QCredentialHistory");
            HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });
        }
    }
}
