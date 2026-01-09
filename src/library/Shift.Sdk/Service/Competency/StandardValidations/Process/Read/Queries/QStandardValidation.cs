using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardValidation
    {
        public Guid StandardValidationIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset? Expired { get; set; }
        public bool IsValidated { get; set; }
        public DateTimeOffset? Notified { get; set; }
        public DateTimeOffset? SelfAssessmentDate { get; set; }
        public string SelfAssessmentStatus { get; set; }
        public string ValidationComment { get; set; }
        public DateTimeOffset? ValidationDate { get; set; }
        public string ValidationStatus { get; set; }
    }
}
