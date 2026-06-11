using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QMessageConfiguration : EntityTypeConfiguration<QMessage>
    {
        public QMessageConfiguration()
            : this("messages")
        {
        }

        public QMessageConfiguration(string schema)
        {
            ToTable(schema + ".QMessage");
            HasKey(x => x.MessageIdentifier);

            Property(x => x.ContentHtml).IsOptional().IsUnicode(true);
            Property(x => x.ContentText).IsOptional().IsUnicode(true);
            Property(x => x.MessageAttachments).IsOptional().IsUnicode(false);
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.MessageTitle).IsRequired().IsUnicode(true).HasMaxLength(256);
            Property(x => x.MessageType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.SenderIdentifier).IsRequired();
            Property(x => x.SurveyFormIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.LastChangeType).IsRequired().IsUnicode(false).HasMaxLength(100);
        }
    }
}