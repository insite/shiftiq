using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class QueueMailout : Command
    {
        public Guid MailoutId { get; set; }
        public string Recipient { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public QueueMailout(Guid messageId, Guid mailoutId, string recipient, Dictionary<string, string> data)
        {
            AggregateIdentifier = messageId;
            MailoutId = mailoutId;
            Recipient = recipient;
            Data = data;
        }
    }
}
