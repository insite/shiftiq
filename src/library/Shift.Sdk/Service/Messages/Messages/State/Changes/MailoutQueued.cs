using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutQueued : Change
    {
        public Guid MailoutId { get; set; }
        public string Recipient { get; set; }
        public IDictionary<string, string> Data { get; set; }

        public MailoutQueued(Guid mailoutId, string recipient, IDictionary<string, string> data)
        {
            MailoutId = mailoutId;
            Recipient = recipient;
            Data = data;
        }
    }
}
