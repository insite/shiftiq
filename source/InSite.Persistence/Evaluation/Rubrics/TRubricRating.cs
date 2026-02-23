using System;

namespace InSite.Persistence
{
    public class TRubricRating
    {
        public Guid CriterionIdentifier { get; set; }
        public Guid RatingIdentifier { get; set; }
        public string RatingTitle { get; set; }
        public string RatingDescription { get; set; }
        public decimal RatingPoints { get; set; }
        public int RubricSequence { get; set; }

        public virtual TRubricCriterion Criterion { get; set; }
    }
}