using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Messaging;

public class RecipientConfiguration : IEntityTypeConfiguration<RecipientEntity>
{
    public void Configure(EntityTypeBuilder<RecipientEntity> builder)
    {
        builder.ToTable("QRecipient", "communications");
        builder.HasKey(x => x.RecipientIdentifier);

        builder.Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.PersonCode).IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.PersonName).IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.PersonLanguage).IsUnicode(false).HasMaxLength(2);
        builder.Property(x => x.DeliveryStatus).IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.DeliveryError).IsUnicode(false);
    }
}