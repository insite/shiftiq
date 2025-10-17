using System;
using System.Collections.Generic;

using InSite.Application.Banks.Read;

namespace InSite.Application.Records.Read
{
    public class QRubric
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

        public virtual ICollection<QRubricCriterion> RubricCriteria { get; set; } = new HashSet<QRubricCriterion>();
        public virtual ICollection<QBankQuestion> BankQuestions { get; set; } = new HashSet<QBankQuestion>();
    }
}
