using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class XMessageConfiguration : EntityTypeConfiguration<VMessage>
    {
        public XMessageConfiguration() : this("messages") { }

        public XMessageConfiguration(string schema)
        {
            ToTable(schema + ".VMessage");
            HasKey(x => x.MessageIdentifier);

            Property(x => x.ContentHtml).IsOptional().IsUnicode(true);
            Property(x => x.ContentText).IsOptional().IsUnicode(true);
            Property(x => x.LinkCount).IsOptional();
            Property(x => x.MailoutCount).IsOptional();
            Property(x => x.MessageAttachments).IsOptional().IsUnicode(false);
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.MessageName).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.MessageTitle).IsRequired().IsUnicode(true).HasMaxLength(256);
            Property(x => x.MessageType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.SenderEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.SenderIdentifier).IsRequired();
            Property(x => x.SenderName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.SubscriberGroupCount).IsOptional();
            Property(x => x.SubscriberUserCount).IsOptional();
            Property(x => x.SurveyFormIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
