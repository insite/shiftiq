using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class ArchivedFollowerConfiguration : EntityTypeConfiguration<ArchivedFollower>
    {
        public ArchivedFollowerConfiguration() : this("messages") { }

        public ArchivedFollowerConfiguration(string schema)
        {
            ToTable(schema + ".ArchivedFollower");
            HasKey(x => new { x.MessageIdentifier, x.SubscriberIdentifier, x.FollowerIdentifier });
        }
    }
}
