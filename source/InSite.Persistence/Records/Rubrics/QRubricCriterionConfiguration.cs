using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QRubricCriterionConfiguration : EntityTypeConfiguration<QRubricCriterion>
    {
        public QRubricCriterionConfiguration() : this("records") { }

        public QRubricCriterionConfiguration(string schema)
        {
            ToTable("QRubricCriterion", schema);
            HasKey(x => new { x.RubricCriterionIdentifier });

            Property(x => x.CriterionTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CriterionDescription).IsUnicode(false).HasMaxLength(1700);
            Property(x => x.CriterionPoints).HasPrecision(5, 2);

            HasRequired(a => a.Rubric).WithMany(b => b.RubricCriteria).HasForeignKey(c => c.RubricIdentifier);
        }
    }
}
