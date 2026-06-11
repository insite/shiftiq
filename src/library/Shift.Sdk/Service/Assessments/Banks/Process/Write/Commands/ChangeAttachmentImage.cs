using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeAttachmentImage : Command
    {
        public Guid Attachment { get; set; }
        public Guid? FileIdentifier { get; set; }
        public Guid Upload { get; set; }
        public Guid Author { get; set; }
        public ImageDimension ActualDimension { get; set; }

        public ChangeAttachmentImage(Guid bank, Guid attachment, Guid? fileIdentifier, Guid upload, Guid author, ImageDimension actual)
        {
            AggregateIdentifier = bank;
            Attachment = attachment;
            FileIdentifier = fileIdentifier;
            Upload = upload;
            Author = author;
            ActualDimension = actual;
        }
    }
}
