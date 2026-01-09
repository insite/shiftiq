using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutCompleted : Change
    {
        public Guid MailoutIdentifier { get; set; }

        public MailoutCompleted(Guid mailoutIdentifier)
        {
            MailoutIdentifier = mailoutIdentifier;
        }
    }
}
