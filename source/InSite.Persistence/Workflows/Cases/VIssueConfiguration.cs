using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class VIssueConfiguration : EntityTypeConfiguration<VIssue>
    {
        public VIssueConfiguration() : this("issues") { }

        public VIssueConfiguration(string schema)
        {
            ToTable(schema + ".VIssue");
            HasKey(x => x.IssueIdentifier);

            Property(x => x.AdministratorUserIdentifier).IsOptional();
            Property(x => x.AdministratorUserName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.AttachmentCount).IsRequired();
            Property(x => x.CommentCount).IsRequired();
            Property(x => x.IssueClosed).IsOptional();
            Property(x => x.IssueClosedBy).IsOptional();
            Property(x => x.IssueDescription).IsOptional().IsUnicode(false).HasMaxLength(6400);
            Property(x => x.IssueEmployerGroupIdentifier).IsOptional();
            Property(x => x.IssueEmployerGroupName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.IssueEmployerGroupParentGroupIdentifier).IsOptional();
            Property(x => x.IssueEmployerGroupParentGroupName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.IssueNumber).IsRequired();
            Property(x => x.IssueOpened).IsRequired();
            Property(x => x.IssueOpenedBy).IsOptional();
            Property(x => x.IssueReported).IsOptional();
            Property(x => x.IssueSource).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.IssueStatusCategory).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.IssueStatusEffective).IsOptional();
            Property(x => x.IssueStatusIdentifier).IsOptional();
            Property(x => x.IssueStatusName).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.IssueStatusSequence).IsOptional();
            Property(x => x.IssueTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.IssueType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.LastChangeTime).IsRequired();
            Property(x => x.LastChangeType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsRequired();
            Property(x => x.LastChangeUserName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.LawyerUserIdentifier).IsOptional();
            Property(x => x.LawyerUserName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.OwnerUserIdentifier).IsOptional();
            Property(x => x.OwnerUserName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.PersonCount).IsRequired();
            Property(x => x.ResponseSessionIdentifier).IsOptional();
            Property(x => x.TopicAccountStatus).IsOptional().IsUnicode(false);
            Property(x => x.TopicEmployerGroupName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.TopicGroupNames).IsOptional().IsUnicode(true);
            Property(x => x.TopicUserIdentifier).IsOptional();
            Property(x => x.TopicUserName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.TopicUserEmail).IsOptional().IsUnicode(false).HasMaxLength(254);

            HasOptional(a => a.Administrator).WithMany(b => b.VAdministratorIssues).HasForeignKey(c => c.AdministratorUserIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Lawyer).WithMany(b => b.VLawyerIssues).HasForeignKey(c => c.LawyerUserIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Owner).WithMany(b => b.VOwnerIssues).HasForeignKey(c => c.OwnerUserIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Topic).WithMany(b => b.VTopicIssues).HasForeignKey(c => c.TopicUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
