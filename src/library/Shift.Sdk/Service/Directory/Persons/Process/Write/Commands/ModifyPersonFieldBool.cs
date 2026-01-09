using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.People.Write
{
    public class ModifyPersonFieldBool : Command
    {
        public PersonField PersonField { get; set; }
        public bool? Value { get; set; }

        public ModifyPersonFieldBool(Guid personId, PersonField personField, bool? value)
        {
            AggregateIdentifier = personId;
            PersonField = personField;
            Value = value;
        }
    }
}
