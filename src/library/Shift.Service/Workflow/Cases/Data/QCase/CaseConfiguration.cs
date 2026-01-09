using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class CaseConfiguration : IEntityTypeConfiguration<CaseEntity>
{
    public void Configure(EntityTypeBuilder<CaseEntity> builder)
    {
        builder.ToTable("QIssue", "issues");
        builder.HasKey(x => new { x.CaseIdentifier });

        builder.Property(x => x.CaseClosed).HasColumnName("IssueClosed");
        builder.Property(x => x.CaseDescription).HasColumnName("IssueDescription").IsUnicode(false).HasMaxLength(6400);
        builder.Property(x => x.CaseIdentifier).HasColumnName("IssueIdentifier").IsRequired();
        builder.Property(x => x.CaseOpened).HasColumnName("IssueOpened").IsRequired();
        builder.Property(x => x.CaseSource).HasColumnName("IssueSource").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CaseStatusCategory).HasColumnName("IssueStatusCategory").IsRequired().IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.CaseType).HasColumnName("IssueType").IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.AdministratorUserIdentifier).HasColumnName("AdministratorUserIdentifier");
        builder.Property(x => x.TopicUserIdentifier).HasColumnName("TopicUserIdentifier");
        builder.Property(x => x.LawyerUserIdentifier).HasColumnName("LawyerUserIdentifier");
        builder.Property(x => x.AttachmentCount).HasColumnName("AttachmentCount").IsRequired();
        builder.Property(x => x.CommentCount).HasColumnName("CommentCount").IsRequired();
        builder.Property(x => x.PersonCount).HasColumnName("PersonCount").IsRequired();
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime").IsRequired();
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsRequired();
        builder.Property(x => x.CaseTitle).HasColumnName("IssueTitle").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.CaseNumber).HasColumnName("IssueNumber").IsRequired();
        builder.Property(x => x.EmployerGroupIdentifier).HasColumnName("EmployerGroupIdentifier");
        builder.Property(x => x.GroupCount).HasColumnName("GroupCount").IsRequired();
        builder.Property(x => x.CaseReported).HasColumnName("IssueReported");
        builder.Property(x => x.CaseOpenedBy).HasColumnName("IssueOpenedBy");
        builder.Property(x => x.CaseClosedBy).HasColumnName("IssueClosedBy");
        builder.Property(x => x.CaseStatusIdentifier).HasColumnName("IssueStatusIdentifier");
        builder.Property(x => x.CaseStatusEffective).HasColumnName("IssueStatusEffective");
        builder.Property(x => x.OwnerUserIdentifier).HasColumnName("OwnerUserIdentifier");
        builder.Property(x => x.ResponseSessionIdentifier).HasColumnName("ResponseSessionIdentifier");

    }
}