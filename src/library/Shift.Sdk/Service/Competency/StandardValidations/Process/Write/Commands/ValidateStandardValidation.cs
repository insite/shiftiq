using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class ValidateStandardValidation : Command
    {
        public Guid LogId { get; set; }
        public bool IsValidated { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }

        public ValidateStandardValidation(Guid standardValidationId, Guid logId, bool isValidated, string status, string comment)
        {
            AggregateIdentifier = standardValidationId;
            LogId = logId;
            IsValidated = isValidated;
            Status = status;
            Comment = comment;
        }
    }
}
