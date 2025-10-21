using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class XMailoutConfiguration : EntityTypeConfiguration<VMailout>
    {
        public XMailoutConfiguration() : this("messages") { }

        public XMailoutConfiguration(string schema)
        {
            ToTable(schema + ".VMailout");
            HasKey(x => x.MailoutIdentifier);

            Property(x => x.ContentSubject).IsRequired().HasMaxLength(180);
            Property(x => x.MailoutError).IsUnicode(false);
            Property(x => x.MailoutStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(180);
            Property(x => x.MessageType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.SenderEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.SenderName).IsUnicode(false).HasMaxLength(100);
            Property(x => x.SenderType).IsUnicode(false).HasMaxLength(50);
        }
    }
}
