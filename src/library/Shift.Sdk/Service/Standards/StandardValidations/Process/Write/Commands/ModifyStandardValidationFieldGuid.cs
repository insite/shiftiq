using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationFieldGuid : Command
    {
        public StandardValidationField Field { get; set; }
        public Guid? Value { get; set; }

        public ModifyStandardValidationFieldGuid(Guid standardValidationId, StandardValidationField standardField, Guid? value)
        {
            AggregateIdentifier = standardValidationId;
            Field = standardField;
            Value = value;
        }
    }
}
