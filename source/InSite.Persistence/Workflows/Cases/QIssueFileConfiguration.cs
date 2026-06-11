using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    internal class QIssueFileRequirementConfiguration : EntityTypeConfiguration<QIssueFileRequirement>
    {
        public QIssueFileRequirementConfiguration() : this("issues") { }

        public QIssueFileRequirementConfiguration(string schema)
        {
            ToTable("QIssueFileRequirement", schema);
            HasKey(x => new { x.IssueIdentifier, x.RequestedFileCategory });

            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.RequestedUserIdentifier).IsRequired();
            Property(x => x.RequestedFileCategory).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.RequestedFileSubcategory).IsUnicode(false).HasMaxLength(120);
            Property(x => x.RequestedFileDescription).IsUnicode(false).HasMaxLength(2400);
            Property(x => x.RequestedTime).IsRequired();
            Property(x => x.RequestedFrom).IsRequired().IsUnicode(false).HasMaxLength(50);
        }
    }
}
