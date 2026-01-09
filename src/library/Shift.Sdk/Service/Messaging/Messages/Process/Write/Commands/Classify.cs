using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class Classify : Command
    {
        public Classify(Guid id, string description, string mailoutType, string recipientType, string recipientRole)
        {
            AggregateIdentifier = id;
            MessageDescription = description;
            MailoutType = mailoutType;
            RecipientType = recipientType;
            RecipientRole = recipientRole;
        }

        public string MessageDescription { get; set; }
        public string MailoutType { get; set; }
        public string RecipientType { get; set; }
        public string RecipientRole { get; set; }
    }
}