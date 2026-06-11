using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class QIssueGroupConfiguration : EntityTypeConfiguration<QIssueGroup>
    {
        public QIssueGroupConfiguration() : this("issues") { }

        public QIssueGroupConfiguration(string schema)
        {
            ToTable(schema + ".QIssueGroup");
            HasKey(x => x.JoinIdentifier);

            Property(x => x.JoinIdentifier).IsRequired();
            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.IssueRole).IsRequired().IsUnicode(false).HasMaxLength(20);
        }
    }
}
