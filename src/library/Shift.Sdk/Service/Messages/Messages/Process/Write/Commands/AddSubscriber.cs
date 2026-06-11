using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class AddSubscriber : Command
    {
        public Guid ContactIdentifier { get; set; }
        public string ContactRole { get; set; }
        public bool Validate { get; set; }
        public bool IsGroup { get; set; }

        public AddSubscriber(Guid message, Guid contact, string role, bool validate, bool isGroup)
        {
            AggregateIdentifier = message;
            ContactIdentifier = contact;
            ContactRole = role;
            Validate = validate;
            IsGroup = isGroup;
        }
    }
}
