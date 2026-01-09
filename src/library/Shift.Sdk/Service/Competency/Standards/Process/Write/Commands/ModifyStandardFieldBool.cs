using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFieldBool : Command
    {
        public StandardField Field { get; set; }
        public bool? Value { get; set; }

        public ModifyStandardFieldBool(Guid standardId, StandardField standardField, bool? value)
        {
            AggregateIdentifier = standardId;
            Field = standardField;
            Value = value;
        }
    }
}
