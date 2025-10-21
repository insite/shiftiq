using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class VIssueAttachmentConfiguration : EntityTypeConfiguration<VIssueAttachment>
    {
        public VIssueAttachmentConfiguration() : this("issues") { }

        public VIssueAttachmentConfiguration(string schema)
        {
            ToTable(schema + ".VIssueAttachment");
            HasKey(x => new { x.IssueIdentifier, x.FileName });

            Property(x => x.FileName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.FileIdentifier).IsRequired();
            Property(x => x.FileUploaded).IsRequired();
            Property(x => x.InputterUserIdentifier).IsOptional();
            Property(x => x.InputterUserName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.IssueNumber).IsRequired();
            Property(x => x.IssueTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.IssueType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.TopicUserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.TopicUserIdentifier).IsRequired();
            Property(x => x.TopicUserName).IsRequired().IsUnicode(false).HasMaxLength(120);
        }
    }
}
