using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TRubricCriterionConfiguration : EntityTypeConfiguration<TRubricCriterion>
    {
        public TRubricCriterionConfiguration() : this("records") { }

        public TRubricCriterionConfiguration(string schema)
        {
            ToTable(schema + ".TRubricCriterion");
            HasKey(x => x.CriterionIdentifier);

            Property(x => x.RubricIdentifier).IsRequired();
            Property(x => x.CriterionIdentifier).IsRequired();
            Property(x => x.CriterionTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CriterionDescription).IsOptional().IsUnicode(false).HasMaxLength(1700);
            Property(x => x.CriterionPoints).IsRequired();
            Property(x => x.CriterionSequence).IsRequired();

            HasRequired(a => a.Rubric).WithMany(b => b.Criteria).HasForeignKey(c => c.RubricIdentifier);
        }
    }
}
