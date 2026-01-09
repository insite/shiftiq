using System;

using Shift.Common;

namespace InSite.Persistence
{
    public class StandardValidation : IHasTimestamp
    {
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid StandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid ValidationIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }

        public string SelfAssessmentStatus { get; set; }
        public string ValidationComment { get; set; }
        public string ValidationStatus { get; set; }

        public bool IsValidated { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Expired { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTimeOffset? Notified { get; set; }
        public DateTimeOffset? SelfAssessmentDate { get; set; }
        public DateTimeOffset? ValidationDate { get; set; }

        public virtual Standard Standard { get; set; }
        public virtual User User { get; set; }
        public virtual User Validator { get; set; }
    }
}
