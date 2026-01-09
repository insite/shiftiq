using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class DeleteFileRequirement : Command
    {
        public string RequestedFileCategory { get; set; }

        public DeleteFileRequirement(Guid issue, string requestedFileCategory)
        {
            AggregateIdentifier = issue;
            RequestedFileCategory = requestedFileCategory;
        }
    }
}
