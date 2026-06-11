using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class TRubricCriterion
    {
        public Guid RubricIdentifier { get; set; }
        public Guid CriterionIdentifier { get; set; }
        public string CriterionTitle { get; set; }
        public string CriterionDescription { get; set; }
        public decimal CriterionPoints { get; set; }
        public int CriterionSequence { get; set; }
        public bool IsRange { get; set; }

        public virtual TRubric Rubric { get; set; }

        public virtual ICollection<TRubricRating> Ratings { get; set; } = new HashSet<TRubricRating>();
    }
}
