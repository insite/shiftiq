using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ChangeAttachmentFile : Command
    {
        public string FileName { get; set; }
        public Guid FileIdentifier { get; set; }

        public ChangeAttachmentFile(Guid aggregate, string fileName, Guid fileIdentifier)
        {
            AggregateIdentifier = aggregate;
            FileName = fileName;
            FileIdentifier = fileIdentifier;
        }
    }
}
