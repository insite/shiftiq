using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class XSubscriberPersonConfiguration : EntityTypeConfiguration<XSubscriberPerson>
    {
        public XSubscriberPersonConfiguration() : this("messages") { }

        public XSubscriberPersonConfiguration(string schema)
        {
            ToTable(schema + ".XSubscriberPerson");
            HasKey(x => new { x.UserIdentifier, x.MessageIdentifier });

            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.MessageOrganizationIdentifier).IsRequired();
            Property(x => x.Subscribed).IsRequired();
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}