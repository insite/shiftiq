using System;

namespace Shift.Common.File
{
    public class UploadMetadata
    {
        public string ContainerType { get; set; }
        public Guid ContainerIdentifier { get; set; }

        public int FileCount { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }

        public string OrganizationCode { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string UploadDate { get; set; }
        public string UploadFolder { get; set; }
        public string UploadUrl { get; set; }
        public string UploadUrlReferrer { get; set; }

        public string UserName { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
