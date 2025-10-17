using System;

namespace InSite.Application.Records.Read
{
    public class QRubricRating
    {
        public Guid RubricCriterionIdentifier { get; set; }
        public Guid RubricRatingIdentifier { get; set; }
        public string RatingTitle { get; set; }
        public string RatingDescription { get; set; }
        public decimal RatingPoints { get; set; }
        public int RatingSequence { get; set; }

        public virtual QRubricCriterion RubricCriterion { get; set; }
    }
}
