using System;
using System.Collections.Generic;

namespace InSite.Application.Files.Read
{
    public class TFile
    {
        public Guid UserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string ObjectType { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string FileLocation { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
        public string FileContentType { get; set; }
        public DateTimeOffset FileUploaded { get; set; }
        public string DocumentName { get; set; }
        public string FileDescription { get; set; }
        public string FileCategory { get; set; }
        public string FileSubcategory { get; set; }
        public string FileStatus { get; set; }
        public DateTimeOffset? FileExpiry { get; set; }
        public DateTimeOffset? FileReceived { get; set; }
        public DateTimeOffset? FileAlternated { get; set; }
        public DateTimeOffset? LastActivityTime { get; set; }
        public Guid? LastActivityUserIdentifier { get; set; }
        public DateTimeOffset? ReviewedTime { get; set; }
        public Guid? ReviewedUserIdentifier { get; set; }
        public DateTimeOffset? ApprovedTime { get; set; }
        public Guid? ApprovedUserIdentifier { get; set; }
        public bool AllowLearnerToView { get; set; }

        public ICollection<TFileActivity> FileActivities { get; set; } = new HashSet<TFileActivity>();
        public ICollection<TFileClaim> FileClaims { get; set; } = new HashSet<TFileClaim>();
    }
}