using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class QIssueUserConfiguration : EntityTypeConfiguration<QIssueUser>
    {
        public QIssueUserConfiguration() : this("issues") { }

        public QIssueUserConfiguration(string schema)
        {
            ToTable(schema + ".QIssueUser");
            HasKey(x => x.JoinIdentifier);

            Property(x => x.JoinIdentifier).IsRequired();
            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.IssueRole).IsRequired().IsUnicode(false).HasMaxLength(20);
        }
    }
}
