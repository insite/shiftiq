using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFieldDateOffset : Command
    {
        public StandardField Field { get; set; }
        public DateTimeOffset? Value { get; set; }

        public ModifyStandardFieldDateOffset(Guid standardId, StandardField standardField, DateTimeOffset? value)
        {
            AggregateIdentifier = standardId;
            Field = standardField;
            Value = value;
        }
    }
}
