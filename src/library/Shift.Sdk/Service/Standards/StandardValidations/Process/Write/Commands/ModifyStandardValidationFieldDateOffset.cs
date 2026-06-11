using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationFieldDateOffset : Command
    {
        public StandardValidationField Field { get; set; }
        public DateTimeOffset? Value { get; set; }

        public ModifyStandardValidationFieldDateOffset(Guid standardValidationId, StandardValidationField standardField, DateTimeOffset? value)
        {
            AggregateIdentifier = standardValidationId;
            Field = standardField;
            Value = value;
        }
    }
}
