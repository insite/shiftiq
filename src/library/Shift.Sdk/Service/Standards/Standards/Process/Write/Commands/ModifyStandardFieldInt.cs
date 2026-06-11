using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFieldInt : Command
    {
        public StandardField Field { get; set; }
        public int? Value { get; set; }

        public ModifyStandardFieldInt(Guid standardId, StandardField standardField, int? value)
        {
            AggregateIdentifier = standardId;
            Field = standardField;
            Value = value;
        }
    }
}
