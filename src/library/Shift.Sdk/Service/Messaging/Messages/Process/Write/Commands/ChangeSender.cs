using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class ChangeSender : Command
    {
        public ChangeSender(Guid message, Guid sender)
        {
            AggregateIdentifier = message;

            Sender = sender;
        }

        public Guid Sender { get; set; }
    }
}