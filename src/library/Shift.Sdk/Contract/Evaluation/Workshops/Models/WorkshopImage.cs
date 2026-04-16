using Shift.Common;

namespace Shift.Contract
{
    public class WorkshopImage
    {
        public class AttachmentInfo
        {
            public string Title { get; set; }
            public string Number { get; set; }
            public string Condition { get; set; }
            public string PublicationStatus { get; set; }            
            public string Dimension { get; set; }
        }

        public string FileName { get; set; }
        public string Url { get; set; }
        public EnvironmentName Environment { get; set; }
        public AttachmentInfo Attachment { get; set; }
    }
}
