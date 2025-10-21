using System;

using InSite.Domain.Banks;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Attachments.Utilities
{
    [Serializable]
    public class AttachmentInfo
    {
        public Guid Author { get; set; }
        public string Title { get; set; }
        public string Condition { get; set; }
        public PublicationStatus PublicationStatus { get; set; }
        public AttachmentType Type { get; set; }
        public AttachmentImage Image { get; set; }
        public AttachmentFileInfo File { get; set; }
        public DateTimeOffset Uploaded { get; set; }
    }
}