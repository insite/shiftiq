using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationFieldText : Command
    {
        public StandardValidationField Field { get; set; }
        public string Value { get; set; }

        public ModifyStandardValidationFieldText(Guid standardValidationId, StandardValidationField standardField, string value)
        {
            AggregateIdentifier = standardValidationId;
            Field = standardField;
            Value = value;
        }
    }
}
