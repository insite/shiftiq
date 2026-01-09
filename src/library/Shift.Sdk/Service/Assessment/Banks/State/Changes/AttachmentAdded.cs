using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class AttachmentAdded : Change
    {
        public Guid Attachment { get; set; }
        public int Asset { get; set; }

        public Guid Author { get; set; }
        public ContentTitle Content { get; set; }
        public string Condition { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public AttachmentType Type { get; set; }

        public Guid Upload { get; set; }
        public AttachmentImage Image { get; set; }

        public AttachmentAdded(Guid attachment, int asset,
            Guid author, ContentTitle content, string condition,
            AttachmentType type, Guid upload, AttachmentImage image)
        {
            Attachment = attachment;
            Asset = asset;

            Author = author;
            Content = content;
            Condition = condition;

            Type = type;
            Upload = upload;
            Image = image;
        }
    }
}
