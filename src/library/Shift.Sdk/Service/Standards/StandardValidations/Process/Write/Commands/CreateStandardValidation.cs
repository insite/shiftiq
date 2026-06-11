using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class CreateStandardValidation : Command
    {
        public Guid StandardId { get; set; }
        public Guid UserId { get; set; }

        public CreateStandardValidation(Guid standardValidationId, Guid standardId, Guid userId)
        {
            AggregateIdentifier = standardValidationId;
            StandardId = standardId;
            UserId = userId;
        }
    }
}
