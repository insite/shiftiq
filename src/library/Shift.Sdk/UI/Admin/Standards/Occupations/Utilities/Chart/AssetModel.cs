using System;

namespace Shift.Sdk.UI
{
    public class AssetModel
    {
        public AssetModel(AssetModel model)
        {
            AssetId = model.AssetId;
            AssetCode = model.AssetCode;
            AssetNumber = model.AssetNumber;
            AssetThumbprint = model.AssetThumbprint;
            AssetTitle = model.AssetTitle;
        }

        public AssetModel(Guid id, string code, int number, Guid thumbprint, string title)
        {
            AssetId = id;
            AssetCode = code;
            AssetNumber = number;
            AssetThumbprint = thumbprint;
            AssetTitle = title;
        }

        public Guid AssetId { get; set; }
        public string AssetCode { get; set; }
        public int AssetNumber { get; set; }
        public Guid AssetThumbprint { get; set; }
        public string AssetTitle { get; set; }
    }
}