using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QRubricRatingConfiguration : EntityTypeConfiguration<QRubricRating>
    {
        public QRubricRatingConfiguration() : this("records") { }

        public QRubricRatingConfiguration(string schema)
        {
            ToTable("QRubricRating", schema);
            HasKey(x => new { x.RubricRatingIdentifier });

            Property(x => x.RatingTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RatingDescription).IsUnicode(false).HasMaxLength(800);
            Property(x => x.RatingPoints).HasPrecision(5, 2);

            HasRequired(a => a.RubricCriterion).WithMany(b => b.RubricRatings).HasForeignKey(c => c.RubricCriterionIdentifier);
        }
    }
}
