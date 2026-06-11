using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.People.Write
{
    public class ModifyPersonFieldGuid : Command
    {
        public PersonField PersonField { get; set; }
        public Guid? Value { get; set; }

        public ModifyPersonFieldGuid(Guid personId, PersonField personField, Guid? value)
        {
            AggregateIdentifier = personId;
            PersonField = personField;
            Value = value;
        }
    }
}
