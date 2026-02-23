using System;

namespace Shift.Common.File
{
    public class UploadFileInfo
    {
        public Guid FileId { get; set; }
        public Guid FileIdentifier => FileId;
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
    }
}
