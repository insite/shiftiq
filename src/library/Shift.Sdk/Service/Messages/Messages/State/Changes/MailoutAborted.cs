using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutAborted : Change
    {
        public Guid Mailout { get; set; }
        public string Reason { get; set; }

        public MailoutAborted(Guid mailout, string reason)
        {
            Mailout = mailout;
            Reason = reason;
        }
    }
}
