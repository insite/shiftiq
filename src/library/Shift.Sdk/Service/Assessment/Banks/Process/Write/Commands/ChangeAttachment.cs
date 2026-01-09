using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeAttachment : Command
    {
        public Guid Attachment { get; set; }
        public string Condition { get; set; }
        public ContentTitle Content { get; set; }
        public AttachmentImage Image { get; set; }

        public ChangeAttachment(Guid bank, Guid attachment, string condition, ContentTitle content, AttachmentImage image)
        {
            AggregateIdentifier = bank;
            Attachment = attachment;
            Condition = condition;
            Content = content;
            Image = image;
        }
    }
}
