using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseAttachmentAdded : Change
    {
        public CaseAttachmentAdded(string fileName, string fileType, Guid fileIdentifier, DateTimeOffset posted, Guid poster)
        {
            FileName = fileName;
            FileType = fileType;
            FileIdentifier = fileIdentifier;
            Posted = posted;
            Poster = poster;
        }

        public string FileName { get; set; }
        public string FileType { get; set; }
        public Guid FileIdentifier { get; set; }
        public DateTimeOffset Posted { get; set; }
        public Guid Poster { get; set; }
    }
}