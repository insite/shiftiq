using System;

using Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class RemoveStandardValidation : Command
    {
        public RemoveStandardValidation(Guid standardValidationId)
        {
            AggregateIdentifier = standardValidationId;
        }
    }
}
