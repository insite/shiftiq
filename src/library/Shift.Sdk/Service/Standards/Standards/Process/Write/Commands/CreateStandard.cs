using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Standards.Write
{
    public class CreateStandard : Command
    {
        public string StandardType { get; set; }
        public int AssetNumber { get; set; }
        public int Sequence { get; set; }
        public ContentContainer Content { get; set; }

        public CreateStandard(Guid standardId, string standardType, int assetNumber, int sequence, ContentContainer content)
        {
            AggregateIdentifier = standardId;
            StandardType = standardType;
            AssetNumber = assetNumber;
            Sequence = sequence;
            Content = content;
        }
    }
}
