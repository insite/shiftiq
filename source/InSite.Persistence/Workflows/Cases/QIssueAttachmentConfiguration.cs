using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class QIssueAttachmentConfiguration : EntityTypeConfiguration<QIssueAttachment>
    {
        public QIssueAttachmentConfiguration() : this("issues") { }

        public QIssueAttachmentConfiguration(string schema)
        {
            ToTable("QIssueAttachment", schema);
            HasKey(x => new { x.AttachmentIdentifier });

            Property(x => x.AttachmentIdentifier).IsRequired();
            Property(x => x.FileIdentifier).IsRequired();
            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier);
            Property(x => x.PosterIdentifier).IsRequired();
            Property(x => x.FileName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.FileType).IsUnicode(false).HasMaxLength(20);
            Property(x => x.AttachmentPosted).IsRequired();
        }
    }
}
