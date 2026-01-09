using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class SenderChanged : Change
    {
        public Guid Sender { get; set; }

        public SenderChanged(Guid sender)
        {
            Sender = sender;
        }
    }
}