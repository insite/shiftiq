using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QRecipientConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<QRecipient>
    {
        public QRecipientConfiguration()
        {
            ToTable("QRecipient", "communications");
            HasKey(x => x.RecipientIdentifier);

            Property(x => x.DeliveryError).IsUnicode(false);
            Property(x => x.DeliveryStatus).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PersonCode).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PersonLanguage).IsUnicode(false).HasMaxLength(2);
            Property(x => x.PersonName).IsUnicode(false).HasMaxLength(120);
            Property(x => x.RecipientVariables).IsUnicode(true);
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);

            HasRequired(a => a.Mailout).WithMany(b => b.Recipients).HasForeignKey(c => c.MailoutIdentifier).WillCascadeOnDelete(false);
        }
    }
}