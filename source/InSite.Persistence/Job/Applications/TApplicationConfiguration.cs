using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TApplicationConfiguration : EntityTypeConfiguration<TApplication>
    {
        public TApplicationConfiguration() : this("jobs") { }

        public TApplicationConfiguration(string schema)
        {
            ToTable(schema + ".TApplication");
            HasKey(x => new { x.ApplicationIdentifier });
            Property(x => x.ApplicationIdentifier).IsRequired();
            Property(x => x.CandidateLetter).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CandidateResume).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CandidateUserIdentifier).IsRequired();
            Property(x => x.OpportunityIdentifier).IsRequired();
            Property(x => x.WhenCreated).IsRequired();
            Property(x => x.WhenModified).IsRequired();

            HasRequired(a => a.CandidateUser).WithMany(b => b.Applications).HasForeignKey(c => c.CandidateUserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Opportunity).WithMany(b => b.Applications).HasForeignKey(c => c.OpportunityIdentifier).WillCascadeOnDelete(false);
        }
    }
}
