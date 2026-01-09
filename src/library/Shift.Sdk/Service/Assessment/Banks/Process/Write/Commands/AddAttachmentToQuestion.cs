using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddAttachmentToQuestion : Command
    {
        public Guid Attachment { get; set; }
        public Guid Question { get; set; }

        public AddAttachmentToQuestion(Guid bank, Guid attachment, Guid question)
        {
            AggregateIdentifier = bank;
            Attachment = attachment;
            Question = question;
        }
    }
}
