using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class AddAttachment : Command
    {
        public Attachment Attachment { get; set; }

        public AddAttachment(Guid bank, Attachment attachment)
        {
            AggregateIdentifier = bank;
            Attachment = attachment;
        }
    }
}