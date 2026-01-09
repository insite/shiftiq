using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class QIssueConfiguration : EntityTypeConfiguration<QIssue>
    {
        public QIssueConfiguration() : this("issues") { }

        public QIssueConfiguration(string schema)
        {
            ToTable(schema + ".QIssue");
            HasKey(x => new { x.IssueIdentifier });

            Property(x => x.AdministratorUserIdentifier).IsOptional();
            Property(x => x.AttachmentCount).IsRequired();
            Property(x => x.CommentCount).IsRequired();
            Property(x => x.EmployerGroupIdentifier).IsOptional();
            Property(x => x.GroupCount).IsRequired();
            Property(x => x.IssueClosed).IsOptional();
            Property(x => x.IssueDescription).IsOptional().IsUnicode(false).HasMaxLength(6400);
            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.IssueNumber).IsRequired();
            Property(x => x.IssueOpened).IsRequired();
            Property(x => x.IssueSource).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.IssueReported).IsOptional();
            Property(x => x.IssueStatusCategory).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.IssueStatusIdentifier).IsOptional();
            Property(x => x.IssueTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.IssueType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.LastChangeTime).IsRequired();
            Property(x => x.LastChangeType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsRequired();
            Property(x => x.LawyerUserIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.OwnerUserIdentifier).IsOptional();
            Property(x => x.PersonCount).IsRequired();
            Property(x => x.TopicUserIdentifier).IsOptional();
        }
    }
}
