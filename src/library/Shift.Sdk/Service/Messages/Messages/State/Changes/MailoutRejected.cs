using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutRejected : Change
    {
        public Guid MailoutId { get; set; }
        public string Recipient { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> Data { get; set; }

        public MailoutRejected(Guid mailoutId, string recipient, string description, IDictionary<string, string> data)
        {
            MailoutId = mailoutId;
            Recipient = recipient;
            Description = description;
            Data = data;
        }
    }
}
