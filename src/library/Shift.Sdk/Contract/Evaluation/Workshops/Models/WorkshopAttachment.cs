using System;

using Shift.Constant;

namespace Shift.Contract
{
    public class WorkshopAttachment
    {
        public Guid AttachmentId { get; set; }
        public AttachmentType AttachmentType { get; set; }
        public int AssetNumber { get; set; }
        public int AssetVersion { get; set; }
        public string Title { get; set; }
        public string Condition { get; set; }
        public string PublicationStatus { get; set; }
        public int QuestionCount { get; set; }
        public DateTimeOffset PostedOn { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileSize { get; set; }
        public string AuthorName { get; set; }
        public int ChangeCount { get; set; }

        // Image-specific
        public string ImageResolution { get; set; }
        public string[] ImageDimensions { get; set; }
        public string Color { get; set; }
    }
}
