using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.People.Write
{
    public class ModifyPersonFieldText : Command
    {
        public PersonField PersonField { get; set; }
        public string Value { get; set; }

        public ModifyPersonFieldText(Guid personId, PersonField personField, string value)
        {
            AggregateIdentifier = personId;
            PersonField = personField;
            Value = value;
        }
    }
}
