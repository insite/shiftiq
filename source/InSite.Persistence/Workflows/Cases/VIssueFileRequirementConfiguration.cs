using System.Data.Entity.ModelConfiguration;

using InSite.Application.Issues.Read;

namespace InSite.Persistence
{
    class VIssueFileRequirementConfiguration : EntityTypeConfiguration<VIssueFileRequirement>
    {
        public VIssueFileRequirementConfiguration() : this("issues") { }

        public VIssueFileRequirementConfiguration(string schema)
        {
            ToTable(schema + ".VIssueFileRequirement");
            HasKey(x => new { x.IssueIdentifier, x.RequestedFileCategory });

            HasRequired(a => a.Issue).WithMany(b => b.IssueFileRequirements).HasForeignKey(c => c.IssueIdentifier).WillCascadeOnDelete(false);
        }
    }
}
