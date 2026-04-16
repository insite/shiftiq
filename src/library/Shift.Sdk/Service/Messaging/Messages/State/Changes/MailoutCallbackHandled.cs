using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutCallbackHandled : Change
    {
        public Guid Mailout { get; set; }
        public Guid CallbackId { get; set; }
        public string Recipient { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public IReadOnlyDictionary<string, string> Data { get; set; }

        public MailoutCallbackHandled(Guid mailout, Guid callbackId, string recipient, DateTime timestamp,
            string status, Dictionary<string, string> data)
        {
            Mailout = mailout;
            CallbackId = callbackId;
            Recipient = recipient;
            Timestamp = timestamp;
            Status = status;
            Data = data.IsEmpty() ? null : new ReadOnlyDictionary<string, string>(data.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
