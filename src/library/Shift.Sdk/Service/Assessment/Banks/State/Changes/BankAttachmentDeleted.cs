using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankAttachmentDeleted : Change
    {
        public Guid Attachment { get; set; }

        public BankAttachmentDeleted(Guid attachment)
        {
            Attachment = attachment;
        }
    }
}
