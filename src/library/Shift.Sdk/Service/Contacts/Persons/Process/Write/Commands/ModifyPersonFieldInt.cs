using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.People.Write
{
    public class ModifyPersonFieldInt : Command
    {
        public PersonField PersonField { get; set; }
        public int? Value { get; set; }

        public ModifyPersonFieldInt(Guid personId, PersonField personField, int? value)
        {
            AggregateIdentifier = personId;
            PersonField = personField;
            Value = value;
        }
    }
}
