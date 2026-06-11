using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class AttachmentChanged : Change
    {
        public Guid Attachment { get; set; }
        public string Condition { get; set; }
        public ContentTitle Content { get; set; }
        public AttachmentImage Image { get; set; }

        public AttachmentChanged(Guid attachment, string condition, ContentTitle content, AttachmentImage image)
        {
            Attachment = attachment;
            Condition = condition;
            Content = content;
            Image = image;
        }
    }
}
