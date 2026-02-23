using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TRubricConfiguration : EntityTypeConfiguration<TRubric>
    {
        public TRubricConfiguration() : this("records") { }

        public TRubricConfiguration(string schema)
        {
            ToTable(schema + ".TRubric");
            HasKey(x => x.RubricIdentifier);

            Property(x => x.RubricIdentifier).IsRequired();
            Property(x => x.RubricTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RubricTitle).IsOptional().IsUnicode(false);
            Property(x => x.RubricPoints).IsRequired();
        }
    }
}
