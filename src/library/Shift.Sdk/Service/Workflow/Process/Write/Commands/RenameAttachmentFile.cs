using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class RenameAttachmentFile : Command
    {
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }

        public RenameAttachmentFile(Guid aggregate, string oldFileName, string newFileName)
        {
            AggregateIdentifier = aggregate;
            OldFileName = oldFileName;
            NewFileName = newFileName;
        }
    }
}
