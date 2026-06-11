using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationStatus : Command
    {
        public Guid LogId { get; set; }
        public bool IsValidated { get; set; }
        public string SelfAssessmentStatus { get; set; }
        public string ValidationStatus { get; set; }
        public string ValidationComment { get; set; }

        public ModifyStandardValidationStatus(Guid standardValidationId, Guid logId, bool isValidated, string selfAssessmentStatus, string validationStatus, string validationComment)
        {
            AggregateIdentifier = standardValidationId;
            LogId = logId;
            IsValidated = isValidated;
            SelfAssessmentStatus = selfAssessmentStatus;
            ValidationStatus = validationStatus;
            ValidationComment = validationComment;
        }
    }
}
