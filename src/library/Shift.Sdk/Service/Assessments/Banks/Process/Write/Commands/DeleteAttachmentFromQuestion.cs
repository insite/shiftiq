using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteAttachmentFromQuestion : Command
    {
        public Guid Attachment { get; set; }
        public Guid Question { get; set; }

        public DeleteAttachmentFromQuestion(Guid bank, Guid attachment, Guid question)
        {
            AggregateIdentifier = bank;
            Attachment = attachment;
            Question = question;
        }
    }
}
