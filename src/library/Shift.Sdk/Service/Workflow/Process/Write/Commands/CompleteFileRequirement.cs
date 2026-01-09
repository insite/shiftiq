using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class CompleteFileRequirement : Command
    {
        public string RequestedFileCategory { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Guid FileIdentifier { get; set; }
        public DateTimeOffset Posted { get; set; }
        public Guid Poster { get; set; }

        public CompleteFileRequirement(Guid issue, string requestedFileCategory, string fileName, string fileType, Guid fileIdentifier, DateTimeOffset posted, Guid poster)
        {
            AggregateIdentifier = issue;
            RequestedFileCategory = requestedFileCategory;
            FileName = fileName;
            FileType = fileType;
            FileIdentifier = fileIdentifier;
            Posted = posted;
            Poster = poster;
        }
    }
}