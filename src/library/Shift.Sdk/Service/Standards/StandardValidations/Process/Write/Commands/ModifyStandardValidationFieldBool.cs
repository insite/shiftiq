using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationFieldBool : Command
    {
        public StandardValidationField Field { get; set; }
        public bool? Value { get; set; }

        public ModifyStandardValidationFieldBool(Guid standardValidationId, StandardValidationField standardField, bool? value)
        {
            AggregateIdentifier = standardValidationId;
            Field = standardField;
            Value = value;
        }
    }
}
