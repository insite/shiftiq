using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardClassificationConfiguration : EntityTypeConfiguration<StandardClassification>
    {
        public StandardClassificationConfiguration() : this("standards") { }

        public StandardClassificationConfiguration(string schema)
        {
            ToTable(schema + ".StandardClassification");
            HasKey(x => new { x.CategoryIdentifier,x.StandardIdentifier });

            Property(x => x.CategoryIdentifier).IsRequired();
            Property(x => x.ClassificationSequence).IsOptional();
            Property(x => x.StandardIdentifier).IsRequired();
        }
    }
}
