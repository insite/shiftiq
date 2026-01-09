using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutStarted : Change
    {
        public Guid MailoutIdentifier { get; set; }

        public MailoutStarted(Guid mailoutIdentifier)
        {
            MailoutIdentifier = mailoutIdentifier;
        }
    }
}