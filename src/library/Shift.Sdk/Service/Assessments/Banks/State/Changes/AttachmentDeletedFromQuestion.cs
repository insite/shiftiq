using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class AttachmentDeletedFromQuestion : Change
    {
        public Guid Attachment { get; set; }
        public Guid Question { get; set; }

        public AttachmentDeletedFromQuestion(Guid attachment, Guid question)
        {
            Attachment = attachment;
            Question = question;
        }
    }
}
