using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class AttachmentImageChanged : Change
    {
        public Guid Attachment { get; set; }
        public Guid? FileIdentifier { get; set; }
        public Guid Upload { get; set; }
        public Guid Author { get; set; }
        public ImageDimension ActualDimension { get; set; }

        public AttachmentImageChanged(Guid attachment, Guid? fileIdentifier, Guid upload, Guid author, ImageDimension actualDimension)
        {
            Attachment = attachment;
            FileIdentifier = fileIdentifier;
            Upload = upload;
            Author = author;
            ActualDimension = actualDimension;
        }
    }
}
