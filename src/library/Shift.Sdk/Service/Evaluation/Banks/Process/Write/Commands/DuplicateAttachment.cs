using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DuplicateAttachment : Command
    {
        public Guid SourceAttachment { get; set; }
        
        public Guid DestinationAttachment { get; set; }
        public int DestinationAsset { get; set; }
        public int DestinationVersion { get; set; }

        public DuplicateAttachment(
            Guid bank, 
            Guid sourceAttachment, 
            Guid destinationAttachment, int destinationAsset, int destinationVersion)
        {
            AggregateIdentifier = bank;
            SourceAttachment = sourceAttachment;
            DestinationAttachment = destinationAttachment;
            DestinationAsset = destinationAsset;
            DestinationVersion = destinationVersion;
        }
    }
}
