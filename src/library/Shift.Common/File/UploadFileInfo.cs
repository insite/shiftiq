using System;

namespace Shift.Common.File
{
    public class UploadFileInfo
    {
        public Guid FileIdentifier { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
    }
}
