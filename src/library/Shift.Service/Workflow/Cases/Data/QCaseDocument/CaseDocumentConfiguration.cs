using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class CaseDocumentConfiguration : IEntityTypeConfiguration<CaseDocumentEntity>
{
    public void Configure(EntityTypeBuilder<CaseDocumentEntity> builder) 
    {
        builder.ToTable("QIssueAttachment", "issues");
        builder.HasKey(x => new { x.AttachmentIdentifier });
            
        builder.Property(x => x.FileName).HasColumnName("FileName").IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.AttachmentPosted).HasColumnName("AttachmentPosted").IsRequired();
        builder.Property(x => x.FileType).HasColumnName("FileType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.IssueIdentifier).HasColumnName("IssueIdentifier").IsRequired();
        builder.Property(x => x.PosterIdentifier).HasColumnName("PosterIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.FileIdentifier).HasColumnName("FileIdentifier").IsRequired();
        builder.Property(x => x.AttachmentIdentifier).HasColumnName("AttachmentIdentifier").IsRequired();

    }
}