using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class CaseDocumentRequestConfiguration : IEntityTypeConfiguration<CaseDocumentRequestEntity>
{
    public void Configure(EntityTypeBuilder<CaseDocumentRequestEntity> builder)
    {
        builder.ToTable("QIssueFileRequirement", "issues");
        builder.HasKey(x => new { x.CaseIdentifier, x.RequestedFileCategory });

        builder.Property(x => x.CaseIdentifier).HasColumnName("IssueIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.RequestedFileCategory).HasColumnName("RequestedFileCategory").IsRequired().IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.RequestedTime).HasColumnName("RequestedTime").IsRequired();
        builder.Property(x => x.RequestedUserIdentifier).HasColumnName("RequestedUserIdentifier").IsRequired();
        builder.Property(x => x.RequestedFrom).HasColumnName("RequestedFrom").IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.RequestedFileSubcategory).HasColumnName("RequestedFileSubcategory").IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.RequestedFileDescription).HasColumnName("RequestedFileDescription").IsUnicode(false).HasMaxLength(2400);

    }
}