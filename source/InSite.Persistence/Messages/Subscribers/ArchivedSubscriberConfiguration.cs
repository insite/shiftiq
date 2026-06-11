using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class ArchivedSubscriberConfiguration : EntityTypeConfiguration<ArchivedSubscriber>
    {
        public ArchivedSubscriberConfiguration() : this("messages") { }

        public ArchivedSubscriberConfiguration(string schema)
        {
            ToTable(schema + ".ArchivedSubscriber");
            HasKey(x => new { x.MessageIdentifier, x.SubscriberIdentifier });
        }
    }
}
