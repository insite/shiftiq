using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TRubricRatingConfiguration : EntityTypeConfiguration<TRubricRating>
    {
        public TRubricRatingConfiguration() : this("records") { }

        public TRubricRatingConfiguration(string schema)
        {
            ToTable(schema + ".TRubricRating");
            HasKey(x => x.RatingIdentifier);

            Property(x => x.CriterionIdentifier).IsRequired();
            Property(x => x.RatingIdentifier).IsRequired();
            Property(x => x.RatingTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RatingDescription).IsOptional().IsUnicode(false).HasMaxLength(800);
            Property(x => x.RatingPoints).IsRequired();

            HasRequired(a => a.Criterion).WithMany(b => b.Ratings).HasForeignKey(c => c.CriterionIdentifier);
        }
    }
}
