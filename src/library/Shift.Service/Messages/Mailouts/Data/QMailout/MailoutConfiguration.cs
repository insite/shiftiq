using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Messaging;

public class MailoutConfiguration : IEntityTypeConfiguration<MailoutEntity>
{
    public void Configure(EntityTypeBuilder<MailoutEntity> builder)
    {
        builder.ToTable("QMailout", "communications");
        builder.HasKey(x => x.MailoutIdentifier);

        builder.Property(x => x.SenderStatus).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.SenderType).IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.MessageType).IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(180);
        builder.Property(x => x.ContentPriority).IsUnicode(false).HasMaxLength(6);
        builder.Property(x => x.ContentSubject).IsRequired().HasMaxLength(180);
        builder.Property(x => x.MailoutStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.MailoutStatusCode).IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.MailoutStatusDescription).IsUnicode(false);
        builder.Property(x => x.MailoutError).IsUnicode(false);
        builder.Property(x => x.RecipientEmailsTo).IsUnicode(false);
        builder.Property(x => x.RecipientEmailsCc).IsUnicode(false);
        builder.Property(x => x.RecipientEmailsBcc).IsUnicode(false);
        builder.Property(x => x.RecipientIdentifiersTo).IsUnicode(false);
        builder.Property(x => x.RecipientIdentifiersCc).IsUnicode(false);
        builder.Property(x => x.RecipientIdentifiersBcc).IsUnicode(false);
        builder.Property(x => x.RecipientListTo).IsUnicode(false);
        builder.Property(x => x.RecipientListCc).IsUnicode(false);
        builder.Property(x => x.RecipientListBcc).IsUnicode(false);
    }
}