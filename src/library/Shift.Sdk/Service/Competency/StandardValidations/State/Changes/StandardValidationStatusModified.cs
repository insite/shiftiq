using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationStatusModified : Change
    {
        public Guid LogId { get; }
        public bool IsValidated { get; }
        public string SelfAssessmentStatus { get; }
        public string ValidationStatus { get; }
        public string ValidationComment { get; }

        public StandardValidationStatusModified(Guid logId, bool isValidated, string selfAssessmentStatus, string validationStatus, string validationComment)
        {
            LogId = logId;
            IsValidated = isValidated;
            SelfAssessmentStatus = selfAssessmentStatus;
            ValidationStatus = validationStatus;
            ValidationComment = validationComment;
        }
    }
}
