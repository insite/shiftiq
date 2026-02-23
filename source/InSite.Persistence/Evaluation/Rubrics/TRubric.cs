using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Persistence
{
    public class TRubric : IHasTimestamp
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid RubricIdentifier { get; set; }
        public string RubricTitle { get; set; }
        public string RubricDescription { get; set; }
        public decimal RubricPoints { get; set; }

        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual ICollection<TRubricCriterion> Criteria { get; set; } = new HashSet<TRubricCriterion>();
        public virtual ICollection<TRubricConnection> Connections { get; set; } = new HashSet<TRubricConnection>();
    }
}
