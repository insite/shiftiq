using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutCancelled : Change
    {
        public Guid MailoutIdentifier { get; set; }

        public MailoutCancelled(Guid mailoutIdentifier)
        {
            MailoutIdentifier = mailoutIdentifier;
        }
    }
}
