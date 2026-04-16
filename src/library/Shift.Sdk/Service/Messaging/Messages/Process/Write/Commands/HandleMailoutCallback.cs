using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class HandleMailoutCallback : Command
    {
        public Guid Mailout { get; set; }
        public Guid CallbackId { get; set; }
        public string Recipient { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public HandleMailoutCallback(
            Guid message, Guid mailout, Guid callbackId, string recipient, DateTime timestamp,
            string status, Dictionary<string, string> data)
        {
            AggregateIdentifier = message;
            Mailout = mailout;
            CallbackId = callbackId;
            Recipient = recipient;
            Timestamp = timestamp;
            Status = status;
            Data = data;
        }
    }
}
