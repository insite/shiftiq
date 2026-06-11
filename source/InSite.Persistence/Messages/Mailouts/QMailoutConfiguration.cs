using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QMailoutConfiguration : EntityTypeConfiguration<QMailout>
    {
        public QMailoutConfiguration()
        {
            ToTable("QMailout", "communications");
            HasKey(x => x.MailoutIdentifier);

            Property(x => x.ContentSubject).IsRequired().HasMaxLength(180);
            Property(x => x.MailoutError).IsUnicode(false);
            Property(x => x.MailoutStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.MailoutStatusCode).IsUnicode(false).HasMaxLength(10);
            Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(180);
            Property(x => x.MessageType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.SenderStatus).IsUnicode(false).HasMaxLength(100);
            Property(x => x.SenderType).IsUnicode(false).HasMaxLength(50);
        }
    }
}