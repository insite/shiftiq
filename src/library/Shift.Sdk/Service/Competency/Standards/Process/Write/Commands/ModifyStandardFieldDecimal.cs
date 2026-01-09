using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFieldDecimal : Command
    {
        public StandardField Field { get; set; }
        public decimal? Value { get; set; }

        public ModifyStandardFieldDecimal(Guid standardId, StandardField standardField, decimal? value)
        {
            AggregateIdentifier = standardId;
            Field = standardField;
            Value = value;
        }
    }
}
