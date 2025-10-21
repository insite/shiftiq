using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TOpportunityCategoryConfiguration : EntityTypeConfiguration<TOpportunityCategory>
    {
        public TOpportunityCategoryConfiguration() : this("jobs") { }

        public TOpportunityCategoryConfiguration(string schema)
        {
            ToTable(schema + ".TOpportunityCategory");
            HasKey(x => new { x.JunctionIdentifier });
            Property(x => x.CategoryItemIdentifier).IsRequired();
            Property(x => x.JunctionIdentifier).IsRequired();
            Property(x => x.OpportunityIdentifier).IsRequired();
        }
    }
}
