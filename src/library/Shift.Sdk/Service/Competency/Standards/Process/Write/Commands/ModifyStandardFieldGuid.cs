using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFieldGuid : Command
    {
        public StandardField Field { get; set; }
        public Guid? Value { get; set; }

        public ModifyStandardFieldGuid(Guid standardId, StandardField standardField, Guid? value)
        {
            AggregateIdentifier = standardId;
            Field = standardField;
            Value = value;
        }
    }
}
