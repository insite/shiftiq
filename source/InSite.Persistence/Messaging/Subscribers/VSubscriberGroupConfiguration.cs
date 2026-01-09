using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class VSubscriberGroupConfiguration : EntityTypeConfiguration<VSubscriberGroup>
    {
        public VSubscriberGroupConfiguration() : this("messages") { }

        public VSubscriberGroupConfiguration(string schema)
        {
            ToTable(schema + ".VSubscriberGroup");
            HasKey(x => new { x.GroupIdentifier, x.MessageIdentifier });

            Property(x => x.GroupCode).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.MessageOrganizationIdentifier).IsRequired();
            Property(x => x.Subscribed).IsRequired();
        }
    }
}