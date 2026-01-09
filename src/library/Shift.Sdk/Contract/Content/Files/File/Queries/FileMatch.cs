using System;

namespace Shift.Contract
{
    public partial class FileMatch
    {
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationCode { get; set; }
        public string ObjectType { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public string FileLocation { get; set; }
        public string FileName { get; set; }
        public string DocumentName { get; set; }
        public int FileSize { get; set; }
        public DateTimeOffset FileUploaded { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public bool HasClaims { get; set; }
    }
}