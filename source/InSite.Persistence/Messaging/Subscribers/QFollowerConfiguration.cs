using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QFollowerConfiguration : EntityTypeConfiguration<QFollower>
    {
        public QFollowerConfiguration() : this("messages") { }

        public QFollowerConfiguration(string schema)
        {
            ToTable(schema + ".QFollower");
            HasKey(x => x.JoinIdentifier);

            Property(x => x.JoinIdentifier).IsRequired();
            Property(x => x.FollowerIdentifier).IsRequired();
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.SubscriberIdentifier).IsRequired();
        }
    }
}
