using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class AddAttachment : Command
    {
        public AddAttachment(Guid aggregate, string fileName, string fileType, Guid fileIdentifier, DateTimeOffset posted, Guid poster)
        {
            AggregateIdentifier = aggregate;
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
