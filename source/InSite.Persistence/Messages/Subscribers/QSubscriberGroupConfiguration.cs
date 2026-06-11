using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QSubscriberGroupConfiguration : EntityTypeConfiguration<QSubscriberGroup>
    {
        public QSubscriberGroupConfiguration() : this("messages") { }

        public QSubscriberGroupConfiguration(string schema)
        {
            ToTable(schema + ".QSubscriberGroup");
            HasKey(x => new { x.GroupIdentifier, x.MessageIdentifier });
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.Subscribed).IsRequired();
        }
    }
}
