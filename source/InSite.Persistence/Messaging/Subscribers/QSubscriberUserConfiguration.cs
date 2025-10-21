using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QSubscriberUserConfiguration : EntityTypeConfiguration<QSubscriberUser>
    {
        public QSubscriberUserConfiguration() : this("messages") { }

        public QSubscriberUserConfiguration(string schema)
        {
            ToTable(schema + ".QSubscriberUser");
            HasKey(x => new { x.UserIdentifier, x.MessageIdentifier });
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.Subscribed).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
