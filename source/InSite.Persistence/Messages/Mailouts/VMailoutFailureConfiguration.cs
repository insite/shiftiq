using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class VMailoutFailureConfiguration : EntityTypeConfiguration<VMailoutFailure>
    {
        public VMailoutFailureConfiguration() : this("communications") { }

        public VMailoutFailureConfiguration(string schema)
        {
            ToTable("VMailoutFailure", schema);
            HasKey(x => new { x.MailoutIdentifier, x.RecipientIdentifier });

            Property(x => x.MessageType).IsUnicode(false).HasMaxLength(20);
            Property(x => x.MessageName).IsUnicode(false).HasMaxLength(180);
            Property(x => x.MessageSubject).IsRequired().HasMaxLength(180);
            Property(x => x.SenderName).IsUnicode(false).HasMaxLength(100);
            Property(x => x.SenderEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.RecipientName).IsUnicode(false).HasMaxLength(120);
            Property(x => x.PersonName).IsUnicode(false).HasMaxLength(120);
            Property(x => x.DeliveryStatus).IsUnicode(false).HasMaxLength(20);
            Property(x => x.DeliveryError).IsUnicode(false);
        }
    }
}
