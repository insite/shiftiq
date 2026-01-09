using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class XFollowerConfiguration : EntityTypeConfiguration<VFollower>
    {
        public XFollowerConfiguration() : this("messages") { }

        public XFollowerConfiguration(string schema)
        {
            ToTable(schema + ".VFollower");
            HasKey(x => new { x.MessageIdentifier, x.SubscriberIdentifier, x.FollowerIdentifier });

            Property(x => x.FollowerIdentifier).IsRequired();
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.Subscribed).IsRequired();
            Property(x => x.SubscriberEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.SubscriberIdentifier).IsRequired();
            Property(x => x.SubscriberName).IsRequired().IsUnicode(false).HasMaxLength(100);
        }
    }
}
