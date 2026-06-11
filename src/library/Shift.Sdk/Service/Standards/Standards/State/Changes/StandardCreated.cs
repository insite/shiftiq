using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Standards
{
    public class StandardCreated : Change
    {
        public string StandardType { get; }
        public int AssetNumber { get; }
        public int Sequence { get; }
        public ContentContainer Content { get; }

        public StandardCreated(string standardType, int assetNumber, int sequence, ContentContainer content)
        {
            StandardType = standardType;
            AssetNumber = assetNumber;
            Sequence = sequence;
            Content = content;
        }
    }
}
