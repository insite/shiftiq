using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.People.Write
{
    public class FixPersonFieldDateOffset : Command
    {
        public PersonField PersonField { get; set; }
        public DateTimeOffset? Value { get; set; }

        public FixPersonFieldDateOffset(Guid personId, PersonField personField, DateTimeOffset? value)
        {
            AggregateIdentifier = personId;
            PersonField = personField;
            Value = value;
        }
    }
}
