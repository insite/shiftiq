using System;

namespace Shift.Contract
{
    public partial class FileModel
    {
        public Guid? ApprovedUserIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public Guid? LastActivityUserIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid? ReviewedUserIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string DocumentName { get; set; }
        public string FileCategory { get; set; }
        public string FileContentType { get; set; }
        public string FileDescription { get; set; }
        public string FileLocation { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileStatus { get; set; }
        public string FileSubcategory { get; set; }
        public string FileUrl { get; set; }
        public string ObjectType { get; set; }

        public int FileSize { get; set; }

        public DateTimeOffset? ApprovedTime { get; set; }
        public DateTimeOffset? FileAlternated { get; set; }
        public DateTimeOffset? FileExpiry { get; set; }
        public DateTimeOffset? FileReceived { get; set; }
        public DateTimeOffset FileUploaded { get; set; }
        public DateTimeOffset? LastActivityTime { get; set; }
        public DateTimeOffset? ReviewedTime { get; set; }
    }
}