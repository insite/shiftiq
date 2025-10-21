using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    public class VIssueUserConfiguration : EntityTypeConfiguration<VIssueUser>
    {
        public VIssueUserConfiguration() : this("issues") { }

        public VIssueUserConfiguration(string schema)
        {
            ToTable(schema + ".VIssueUser");
            HasKey(x => new { x.IssueIdentifier, x.UserIdentifier, x.IssueRole });

            Property(x => x.IssueIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.IssueRole).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.UserFullName).IsOptional().IsUnicode(false).HasMaxLength(100);

            HasRequired(a => a.VIssue).WithMany(b => b.VUsers).HasForeignKey(c => c.IssueIdentifier).WillCascadeOnDelete(false);
        }
    }
}
