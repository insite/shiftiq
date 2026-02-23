using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class DeleteAttachment : Command
    {
        public string FileName { get; set; }

        public DeleteAttachment(Guid aggregate, string filename)
        {
            AggregateIdentifier = aggregate;
            FileName = filename;
        }
    }
}
