using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.People.Write
{
    public class ModifyPersonFieldDate : Command
    {
        public PersonField PersonField { get; set; }
        public DateTime? Value { get; set; }

        public ModifyPersonFieldDate(Guid personId, PersonField personField, DateTime? value)
        {
            AggregateIdentifier = personId;
            PersonField = personField;
            Value = value;
        }
    }
}
