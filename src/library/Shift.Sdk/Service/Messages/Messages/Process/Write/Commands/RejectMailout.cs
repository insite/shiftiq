using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class RejectMailout : Command
    {
        public Guid MailoutId { get; set; }
        public string Recipient { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public RejectMailout(Guid messageId, Guid mailoutId, string recipient, string description, Dictionary<string, string> data)
        {
            AggregateIdentifier = messageId;
            MailoutId = mailoutId;
            Recipient = recipient;
            Description = description;
            Data = data;
        }
    }
}
