using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    public class QRubricCriterion
    {
        public Guid RubricIdentifier { get; set; }
        public Guid RubricCriterionIdentifier { get; set; }
        public string CriterionTitle { get; set; }
        public string CriterionDescription { get; set; }
        public decimal CriterionPoints { get; set; }
        public int CriterionSequence { get; set; }
        public bool IsRange { get; set; }

        public virtual QRubric Rubric { get; set; }

        public virtual ICollection<QRubricRating> RubricRatings { get; set; } = new HashSet<QRubricRating>();
    }
}
