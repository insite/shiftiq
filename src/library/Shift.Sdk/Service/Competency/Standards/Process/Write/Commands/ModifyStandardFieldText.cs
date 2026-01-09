using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFieldText : Command
    {
        public StandardField Field { get; set; }
        public string Value { get; set; }

        public ModifyStandardFieldText(Guid standardId, StandardField standardField, string value)
        {
            AggregateIdentifier = standardId;
            Field = standardField;
            Value = value;
        }
    }
}
